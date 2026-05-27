import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { AuthFacadeService } from '../../../../core/services/auth/auth-facade.service';
import { ADMIN_ROLE_ID, MEMBER_ROLE_ID, TRAINER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { DialogButton } from '../../models/dialog-config.model';
import { DialogHelperService } from '../../services/dialog-helper.service';
import { UserMembershipsApiService } from '../../../../api-services/user-memberships/user-memberships-api.service';
import {
  GetMyActiveUserMembershipQueryDto,
  ListMyMembershipPurchaseHistoryQueryDto,
} from '../../../../api-services/user-memberships/user-memberships-api.models';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import {
  ListTrainingsQueryDto,
  ListTrainingsRequest,
} from '../../../../api-services/trainings/trainings-api.models';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import {
  ListTrainersQueryDto,
  ListTrainersRequest,
} from '../../../../api-services/trainers/trainers-api.models';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { UpdateUserCommand } from '../../../../api-services/users/users-api.models';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';
import { ListOrdersWithItemsRequest } from '../../../../api-services/orders/orders-api.models';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import {
  ListProductsQueryDto,
  ListProductsRequest,
} from '../../../../api-services/products/products-api.models';
import { TrainingRequestsApiService } from '../../../../api-services/training-requests/training-requests-api.service';
import {
  ListTrainerTrainingRequestQueryDto,
  ListTrainingRequestQueryDto,
  TRAINING_REQUEST_APPROVED,
  TRAINING_REQUEST_PENDING,
  TRAINING_REQUEST_REJECTED,
} from '../../../../api-services/training-requests/training-requests-api.models';

export interface UpcomingTrainingView {
  training: ListTrainingsQueryDto;
  trainerName: string;
  typeLabel: string;
}

export interface PurchaseHistoryRow {
  refId: number;
  sortAt: number;
  status: string;
  typeLabel: string;
  typeClass: string;
  itemName: string;
  price: number;
}

@Component({
  selector: 'app-account-profile',
  standalone: false,
  templateUrl: './account-profile.component.html',
  styleUrl: './account-profile.component.scss',
})
export class AccountProfileComponent implements OnInit {
  profileService = inject(UserProfileService);
  private fb = inject(FormBuilder);
  private translate = inject(TranslateService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private auth = inject(AuthFacadeService);
  private dialog = inject(DialogHelperService);
  private usersApi = inject(UsersApiService);
  private membershipsApi = inject(UserMembershipsApiService);
  private trainingsApi = inject(TrainingsApiService);
  private trainersApi = inject(TrainersApiService);
  private ordersApi = inject(OrdersApiService);
  private productsApi = inject(ProductsApiService);
  private trainingRequestsApi = inject(TrainingRequestsApiService);

  loading = true;
  saving = false;
  deletingProfile = false;
  editing = false;
  saveError: string | null = null;

  activeMembership: GetMyActiveUserMembershipQueryDto | null = null;
  trainerRecord: ListTrainersQueryDto | null = null;
  upcomingTrainings: UpcomingTrainingView[] = [];
  memberBookings: ListTrainingRequestQueryDto[] = [];
  pendingTrainerRequests: ListTrainerTrainingRequestQueryDto[] = [];
  bookingActionId: number | null = null;
  purchases: PurchaseHistoryRow[] = [];

  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.minLength(2)]],
    lastName: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: [''],
    bio: [''],
    experienceYears: [0, [Validators.min(0)]],
  });

  ngOnInit(): void {
    const init = () => {
      if (this.profileService.profile()) {
        this.loadPageData();
        return;
      }
      this.profileService.loadProfile().subscribe(() => this.loadPageData());
    };
    init();

    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(() => {
        if (this.router.url.includes('/profile')) {
          this.loadPageData();
        }
      });
  }

  get isMember(): boolean {
    return this.profileService.profile()?.roleId === MEMBER_ROLE_ID;
  }

  get isTrainer(): boolean {
    return this.profileService.profile()?.roleId === TRAINER_ROLE_ID;
  }

  get isAdmin(): boolean {
    return this.profileService.profile()?.roleId === ADMIN_ROLE_ID;
  }

  get canDeleteProfile(): boolean {
    return this.isMember || this.isTrainer;
  }

  displayName(): string {
    const p = this.profileService.profile();
    if (!p) return '';
    if (this.editing) {
      const v = this.form.getRawValue();
      return `${v.firstName ?? ''} ${v.lastName ?? ''}`.trim();
    }
    return p.fullName;
  }

  avatarLetters(): string {
    const p = this.profileService.profile();
    if (!p) return '?';
    if (this.editing) {
      const v = this.form.getRawValue();
      return `${(v.firstName || '?')[0]}${(v.lastName || '?')[0]}`.toUpperCase();
    }
    return `${p.firstName[0] ?? ''}${p.lastName[0] ?? ''}`.toUpperCase();
  }

  roleLabel(roleId: number): string {
    if (roleId === ADMIN_ROLE_ID) return this.translate.instant('CLIENT.PROFILE.ROLE_ADMIN');
    if (roleId === TRAINER_ROLE_ID) return this.translate.instant('CLIENT.PROFILE.ROLE_TRAINER');
    if (roleId === MEMBER_ROLE_ID) return this.translate.instant('CLIENT.PROFILE.ROLE_MEMBER');
    return this.translate.instant('CLIENT.PROFILE.ROLE_UNKNOWN', { id: roleId });
  }

  planDurationLabel(days: number): string {
    return `${days} ${this.translate.instant('COMMON.DAYS')}`;
  }

  renewMembership(): void {
    this.router.navigate(['/client/memberships']);
  }

  bookingStatusLabel(status: number): string {
    if (status === TRAINING_REQUEST_PENDING)
      return this.translate.instant('CLIENT.PROFILE.BOOKING_PENDING');
    if (status === TRAINING_REQUEST_APPROVED)
      return this.translate.instant('CLIENT.PROFILE.BOOKING_APPROVED');
    if (status === TRAINING_REQUEST_REJECTED)
      return this.translate.instant('CLIENT.PROFILE.BOOKING_REJECTED');
    return String(status);
  }

  approveBooking(id: number): void {
    if (this.bookingActionId != null) return;
    this.bookingActionId = id;
    this.trainingRequestsApi.approve(id).subscribe({
      next: () => {
        this.bookingActionId = null;
        this.toaster.success(this.translate.instant('CLIENT.PROFILE.BOOKING_ACCEPTED'));
        this.loadPageData();
      },
      error: (err) => {
        this.bookingActionId = null;
        this.toaster.error(err?.error?.message ?? err?.error?.title ?? 'Error');
      },
    });
  }

  rejectBooking(id: number): void {
    if (this.bookingActionId != null) return;
    this.bookingActionId = id;
    this.trainingRequestsApi.reject(id).subscribe({
      next: () => {
        this.bookingActionId = null;
        this.toaster.success(this.translate.instant('CLIENT.PROFILE.BOOKING_DECLINED'));
        this.loadPageData();
      },
      error: (err) => {
        this.bookingActionId = null;
        this.toaster.error(err?.error?.message ?? err?.error?.title ?? 'Error');
      },
    });
  }

  formatBookingTime(startTime: string): string {
    return (startTime ?? '').slice(0, 5);
  }

  startEdit(): void {
    const p = this.profileService.profile();
    if (!p) return;
    this.patchProfileForm(p);
    this.saveError = null;
    this.editing = true;
  }

  cancelEdit(): void {
    this.editing = false;
    this.saveError = null;
    this.form.markAsPristine();
  }

  deleteProfile(): void {
    const p = this.profileService.profile();
    if (!p || !this.canDeleteProfile || this.deletingProfile) return;

    this.dialog
      .confirmDelete(p.fullName, 'CLIENT.PROFILE.DELETE_CONFIRM')
      .subscribe((result) => {
        if (result?.button !== DialogButton.DELETE) return;

        this.deletingProfile = true;
        this.usersApi.delete(p.id).subscribe({
          next: () => {
            this.profileService.clear();
            this.auth.logout().subscribe(() => {
              this.router.navigate(['/auth/login']);
            });
          },
          error: (err) => {
            this.deletingProfile = false;
            this.toaster.error(
              err?.error?.message ??
                err?.error?.title ??
                this.translate.instant('CLIENT.PROFILE.DELETE_ERROR'),
            );
          },
        });
      });
  }

  applyChanges(): void {
    if (this.form.invalid || this.saving) {
      this.form.markAllAsTouched();
      return;
    }

    const p = this.profileService.profile();
    if (!p) return;

    const { firstName, lastName, email, password, bio, experienceYears } = this.form.getRawValue();
    const userPayload: UpdateUserCommand = {
      firstName: firstName ?? '',
      lastName: lastName ?? '',
      email: email ?? '',
      roleId: p.roleId,
      gymId: p.gymId,
    };
    const pwd = (password ?? '').trim();
    if (pwd) userPayload.password = pwd;

    this.saving = true;
    this.saveError = null;

    this.usersApi
      .update(p.id, userPayload)
      .pipe(
        switchMap(() => {
          if (!this.isTrainer || !this.trainerRecord) return of(null);
          return this.trainersApi.update(this.trainerRecord.id, {
            userId: p.id,
            gymId: p.gymId,
            bio: bio ?? '',
            experienceYears: Number(experienceYears) || 0,
          });
        }),
        switchMap(() => this.profileService.loadProfile()),
      )
      .subscribe({
        next: () => {
          this.saving = false;
          this.editing = false;
          this.toaster.success(this.translate.instant('CLIENT.PROFILE.SAVE_SUCCESS'));
          this.loadPageData();
        },
        error: (err) => {
          this.saving = false;
          this.saveError =
            err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('CLIENT.PROFILE.SAVE_ERROR');
        },
      });
  }

  private loadPageData(): void {
    const profile = this.profileService.profile();
    if (!profile) {
      this.loading = false;
      return;
    }

    this.loading = true;

    const gymTrainersReq = new ListTrainersRequest();
    gymTrainersReq.gymId = profile.gymId;
    gymTrainersReq.paging.pageSize = 100;

    const myTrainerReq = new ListTrainersRequest();
    myTrainerReq.userId = profile.id;
    myTrainerReq.gymId = profile.gymId;
    myTrainerReq.paging.pageSize = 1;

    const trainingsReq = new ListTrainingsRequest();
    trainingsReq.paging.pageSize = 100;

    const ordersReq = new ListOrdersWithItemsRequest();
    ordersReq.paging.pageSize = 50;

    const productsReq = new ListProductsRequest();
    productsReq.paging.pageSize = 500;

    const loads: Record<string, any> = {
      gymTrainers: this.trainersApi.list(gymTrainersReq).pipe(catchError(() => of({ items: [] }))),
      myTrainer: this.trainersApi.list(myTrainerReq).pipe(catchError(() => of({ items: [] }))),
      trainings: this.trainingsApi.list(trainingsReq).pipe(catchError(() => of({ items: [] }))),
      orders: this.ordersApi.listWithItems(ordersReq).pipe(catchError(() => of({ items: [] }))),
      products: this.productsApi.list(productsReq).pipe(catchError(() => of({ items: [] }))),
      membership: this.membershipsApi.getMyActive().pipe(catchError(() => of(null))),
      membershipHistory: this.membershipsApi.listMyHistory(),
    };

    if (this.isMember) {
      loads['myBookings'] = this.trainingRequestsApi.listMy().pipe(catchError(() => of([])));
      loads['myTrainings'] = this.trainingsApi.listMy().pipe(catchError(() => of([])));
    }
    if (this.isTrainer) {
      loads['trainerBookings'] = this.trainingRequestsApi
        .listForTrainer(TRAINING_REQUEST_PENDING)
        .pipe(catchError(() => of([])));
    }

    forkJoin(loads)
      .pipe(
        switchMap((data: any) => {
          const trainers = data.gymTrainers.items ?? [];
          if (!trainers.length) {
            return of({ ...data, trainerUsers: new Map<number, string>() });
          }
          return forkJoin<{ trainerId: number; name: string }[]>(
            trainers.map((t: ListTrainersQueryDto) =>
              this.usersApi.getById(t.userId).pipe(
                catchError(() => of(null)),
                map((user) => ({
                  trainerId: t.id,
                  name: user
                    ? `${user.firstName} ${user.lastName}`.trim()
                    : this.translate.instant('CLIENT.PROFILE.TRAINER_FALLBACK', { id: t.id }),
                })),
              ),
            ),
          ).pipe(
            map((pairs) => {
              const trainerUsers = new Map<number, string>();
              pairs.forEach((pair) => trainerUsers.set(pair.trainerId, pair.name));
              return { ...data, trainerUsers };
            }),
          );
        }),
      )
      .subscribe({
        next: (data: any) => {
          this.trainerRecord = data.myTrainer?.items?.[0] ?? null;
          this.patchProfileForm(profile);

          this.activeMembership = this.isMember ? (data.membership ?? null) : null;

          const membershipHistory = (data.membershipHistory ??
            []) as ListMyMembershipPurchaseHistoryQueryDto[];

          const trainerNames: Map<number, string> = data.trainerUsers ?? new Map();
          const now = new Date();
          let trainings = (data.trainings.items ?? []) as ListTrainingsQueryDto[];

          if (this.isTrainer && this.trainerRecord) {
            trainings = trainings.filter((t) => t.trainerId === this.trainerRecord!.id);
            this.pendingTrainerRequests = (data.trainerBookings ??
              []) as ListTrainerTrainingRequestQueryDto[];
          } else {
            this.pendingTrainerRequests = [];
          }

          if (this.isMember) {
            const allBookings = (data.myBookings ?? []) as ListTrainingRequestQueryDto[];
            this.memberBookings = allBookings
              .filter((b) => {
                return b.status === TRAINING_REQUEST_PENDING && this.bookingStartsAt(b) >= now;
              })
              .sort((a, b) => this.bookingStartsAt(a).getTime() - this.bookingStartsAt(b).getTime());
            trainings = (data.myTrainings ?? []) as ListTrainingsQueryDto[];
          } else {
            this.memberBookings = [];
          }

          this.upcomingTrainings = trainings
            .filter((t) => this.trainingStartsAt(t) >= now)
            .sort((a, b) => this.trainingStartsAt(a).getTime() - this.trainingStartsAt(b).getTime())
            .slice(0, 6)
            .map((t) => ({
              training: t,
              trainerName:
                trainerNames.get(t.trainerId) ??
                this.translate.instant('CLIENT.PROFILE.TRAINER_FALLBACK', { id: t.trainerId }),
              typeLabel: this.trainingTypeLabel(t.type),
            }));

          const productMap = new Map<number, ListProductsQueryDto>(
            (data.products.items ?? []).map((prod: ListProductsQueryDto) => [prod.id, prod]),
          );

          this.purchases = [];
          for (const m of membershipHistory) {
            this.purchases.push({
              refId: m.userMembershipId,
              sortAt: new Date(m.purchasedAt).getTime(),
              status: m.isActive
                ? this.translate.instant('CLIENT.PROFILE.STATUS_ACTIVE')
                : this.translate.instant('CLIENT.PROFILE.STATUS_EXPIRED'),
              typeLabel: this.translate.instant('CLIENT.PROFILE.TYPE_MEMBERSHIP'),
              typeClass: 'tag-membership',
              itemName: m.planName,
              price: m.amountPaid,
            });
          }

          if (this.isMember) {
            for (const order of data.orders.items ?? []) {
              const items = order.items ?? [];
              for (const item of items) {
                const productId = (item as { productId?: number }).productId;
                const product = productId ? productMap.get(productId) : undefined;
                const qty = (item as { quantity?: number }).quantity ?? 1;
                const unitPrice = (item as { price?: number }).price ?? 0;
                this.purchases.push({
                  refId: order.id,
                  sortAt: order.id,
                  status: String(order.status ?? ''),
                  typeLabel: this.translate.instant('CLIENT.PROFILE.TYPE_SUPPLEMENT'),
                  typeClass: 'tag-supplement',
                  itemName:
                    product?.name ??
                    this.translate.instant('CLIENT.PROFILE.PRODUCT_FALLBACK', {
                      id: productId ?? item.id,
                    }),
                  price: unitPrice * qty,
                });
              }
              if (!items.length) {
                this.purchases.push({
                  refId: order.id,
                  sortAt: order.id,
                  status: String(order.status ?? ''),
                  typeLabel: this.translate.instant('CLIENT.PROFILE.TYPE_SUPPLEMENT'),
                  typeClass: 'tag-supplement',
                  itemName: `#${order.id}`,
                  price: order.totalAmount ?? 0,
                });
              }
            }

          }

          this.purchases.sort((a, b) => b.sortAt - a.sortAt);
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }

  private trainingTypeLabel(type: number): string {
    if (type === 1) return this.translate.instant('CLIENT.PROFILE.TYPE_INDIVIDUAL');
    if (type === 2) return this.translate.instant('CLIENT.PROFILE.TYPE_GROUP');
    return `${this.translate.instant('CLIENT.TRAINER_HOME.TYPE')} ${type}`;
  }

  private trainingStartsAt(training: ListTrainingsQueryDto): Date {
    return this.dateTimeFromParts(training.date, training.startTime);
  }

  private bookingStartsAt(booking: ListTrainingRequestQueryDto): Date {
    return this.dateTimeFromParts(booking.date, booking.startTime);
  }

  private dateTimeFromParts(dateValue: string, timeValue: string): Date {
    const date = (dateValue ?? '').slice(0, 10);
    const time = (timeValue ?? '00:00:00').slice(0, 8);
    return new Date(`${date}T${time}`);
  }

  private patchProfileForm(profile: NonNullable<ReturnType<UserProfileService['profile']>>): void {
    this.form.patchValue({
      firstName: profile.firstName,
      lastName: profile.lastName,
      email: profile.email,
      password: '',
      bio: this.trainerRecord?.bio ?? '',
      experienceYears: this.trainerRecord?.experienceYears ?? 0,
    });
  }
}
