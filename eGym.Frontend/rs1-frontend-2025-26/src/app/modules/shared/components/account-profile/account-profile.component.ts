import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { ADMIN_ROLE_ID, MEMBER_ROLE_ID, TRAINER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { MembershipPlansApiService } from '../../../../api-services/membership-plans/membership-plans-api.service';
import {
  ListMembershipPlansQueryDto,
  ListMembershipPlansRequest,
} from '../../../../api-services/membership-plans/membership-plans-api.models';
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

export interface UpcomingTrainingView {
  training: ListTrainingsQueryDto;
  trainerName: string;
  typeLabel: string;
}

export interface PurchaseHistoryRow {
  orderId: number;
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
  private usersApi = inject(UsersApiService);
  private plansApi = inject(MembershipPlansApiService);
  private trainingsApi = inject(TrainingsApiService);
  private trainersApi = inject(TrainersApiService);
  private ordersApi = inject(OrdersApiService);
  private productsApi = inject(ProductsApiService);

  loading = true;
  saving = false;
  editing = false;
  saveError: string | null = null;

  membershipPlan: ListMembershipPlansQueryDto | null = null;
  trainerRecord: ListTrainersQueryDto | null = null;
  upcomingTrainings: UpcomingTrainingView[] = [];
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
    if (this.profileService.profile()) {
      this.loadPageData();
      return;
    }
    this.profileService.loadProfile().subscribe(() => this.loadPageData());
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

  planPrice(plan: ListMembershipPlansQueryDto): number {
    const discount = (plan.price * plan.discountPercentage) / 100;
    return Math.round((plan.price - discount) * 100) / 100;
  }

  renewMembership(): void {
    this.router.navigate(['/client']);
  }

  startEdit(): void {
    const p = this.profileService.profile();
    if (!p) return;
    this.form.patchValue({
      firstName: p.firstName,
      lastName: p.lastName,
      email: p.email,
      password: '',
      bio: this.trainerRecord?.bio ?? '',
      experienceYears: this.trainerRecord?.experienceYears ?? 0,
    });
    this.saveError = null;
    this.editing = true;
  }

  cancelEdit(): void {
    this.editing = false;
    this.saveError = null;
    this.form.markAsPristine();
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

    const plansReq = new ListMembershipPlansRequest();
    plansReq.gymId = profile.gymId;
    plansReq.paging.pageSize = 50;

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
    };

    if (this.isMember) {
      loads['plans'] = this.plansApi.list(plansReq).pipe(catchError(() => of({ items: [] })));
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

          if (this.isMember) {
            const plans = data.plans?.items ?? [];
            this.membershipPlan = this.pickMembershipPlan(plans);
          } else {
            this.membershipPlan = null;
          }

          const trainerNames: Map<number, string> = data.trainerUsers ?? new Map();
          const today = new Date().toISOString().slice(0, 10);
          let trainings = (data.trainings.items ?? []) as ListTrainingsQueryDto[];

          if (this.isTrainer && this.trainerRecord) {
            trainings = trainings.filter((t) => t.trainerId === this.trainerRecord!.id);
          }

          this.upcomingTrainings = trainings
            .filter((t) => (t.date ?? '').slice(0, 10) >= today)
            .sort((a, b) => `${a.date}${a.startTime}`.localeCompare(`${b.date}${b.startTime}`))
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
          if (this.isMember) {
            for (const order of data.orders.items ?? []) {
              const items = order.items ?? [];
              for (const item of items) {
                const productId = (item as { productId?: number }).productId;
                const product = productId ? productMap.get(productId) : undefined;
                const qty = (item as { quantity?: number }).quantity ?? 1;
                const unitPrice = (item as { price?: number }).price ?? 0;
                this.purchases.push({
                  orderId: order.id,
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
                  orderId: order.id,
                  status: String(order.status ?? ''),
                  typeLabel: this.translate.instant('CLIENT.PROFILE.TYPE_SUPPLEMENT'),
                  typeClass: 'tag-supplement',
                  itemName: `#${order.id}`,
                  price: order.totalAmount ?? 0,
                });
              }
            }
          }

          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }

  private pickMembershipPlan(
    plans: ListMembershipPlansQueryDto[],
  ): ListMembershipPlansQueryDto | null {
    if (!plans.length) return null;
    const monthly = plans.find((p) => p.durationDays === 30);
    if (monthly) return monthly;
    return [...plans].sort((a, b) => a.durationDays - b.durationDays)[0];
  }

  private trainingTypeLabel(type: number): string {
    if (type === 1) return this.translate.instant('CLIENT.PROFILE.TYPE_INDIVIDUAL');
    if (type === 2) return this.translate.instant('CLIENT.PROFILE.TYPE_GROUP');
    return `${this.translate.instant('CLIENT.TRAINER_HOME.TYPE')} ${type}`;
  }
}
