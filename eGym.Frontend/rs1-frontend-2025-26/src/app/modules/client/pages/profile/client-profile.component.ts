import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserProfileService } from '../../../../core/services/user-profile.service';
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
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';
import {
  ListOrdersWithItemsQueryDto,
  ListOrdersWithItemsRequest,
} from '../../../../api-services/orders/orders-api.models';

export interface UpcomingTrainingView {
  id: number;
  trainerName: string;
  typeLabelKey: string;
  date: string;
}

@Component({
  selector: 'app-client-profile',
  standalone: false,
  templateUrl: './client-profile.component.html',
  styleUrl: './client-profile.component.scss',
})
export class ClientProfileComponent implements OnInit {
  profileService = inject(UserProfileService);
  private translate = inject(TranslateService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);
  private plansApi = inject(MembershipPlansApiService);
  private trainingsApi = inject(TrainingsApiService);
  private trainersApi = inject(TrainersApiService);
  private usersApi = inject(UsersApiService);
  private ordersApi = inject(OrdersApiService);

  loading = true;
  membershipPlan: ListMembershipPlansQueryDto | null = null;
  upcomingTrainings: UpcomingTrainingView[] = [];
  purchases: ListOrdersWithItemsQueryDto[] = [];

  ngOnInit(): void {
    if (!this.profileService.profile()) {
      this.profileService.loadProfile().subscribe(() => this.loadData());
      return;
    }
    this.loadData();
  }

  roleLabel(roleId: number): string {
    if (roleId === ADMIN_ROLE_ID) return this.translate.instant('CLIENT.PROFILE.ROLE_ADMIN');
    if (roleId === TRAINER_ROLE_ID) return this.translate.instant('CLIENT.PROFILE.ROLE_TRAINER');
    if (roleId === MEMBER_ROLE_ID) return this.translate.instant('CLIENT.PROFILE.ROLE_MEMBER');
    return this.translate.instant('CLIENT.PROFILE.ROLE_UNKNOWN', { id: roleId });
  }

  formatDuration(days: number): string {
    return `${days} ${this.translate.instant('COMMON.DAYS')}`;
  }

  membershipPrice(plan: ListMembershipPlansQueryDto): number {
    const discount = (plan.price * plan.discountPercentage) / 100;
    return Math.round((plan.price - discount) * 100) / 100;
  }

  trainingTypeKey(type: number): string {
    return type === 2
      ? 'CLIENT.PROFILE.TRAINING_TYPE_GROUP'
      : 'CLIENT.PROFILE.TRAINING_TYPE_INDIVIDUAL';
  }

  orderServiceLabel(order: ListOrdersWithItemsQueryDto): string {
    const first = order.items?.[0]?.product?.name;
    return first || order.referenceNumber || order.note || `#${order.id}`;
  }

  orderTypeLabel(order: ListOrdersWithItemsQueryDto): string {
    const name = order.items?.[0]?.product?.name?.toLowerCase() ?? '';
    if (name.includes('član') || name.includes('member')) {
      return this.translate.instant('CLIENT.HOME.MEMBERSHIPS');
    }
    return this.translate.instant('CLIENT.PROFILE.ORDER_TYPE_PRODUCT');
  }

  orderTypeClass(order: ListOrdersWithItemsQueryDto): string {
    const name = order.items?.[0]?.product?.name?.toLowerCase() ?? '';
    if (name.includes('član') || name.includes('member')) return 'tag--purple';
    if (name.includes('ishran') || name.includes('nutri')) return 'tag--blue';
    if (name.includes('suplement') || name.includes('whey') || name.includes('protein')) {
      return 'tag--green';
    }
    if (name.includes('tren') || name.includes('coach')) return 'tag--pink';
    return 'tag--gray';
  }

  onEditClick(): void {
    this.snackBar.open(this.translate.instant('CLIENT.PROFILE.EDIT_UNAVAILABLE'), undefined, {
      duration: 3500,
    });
  }

  extendMembership(): void {
    this.router.navigate(['/client']);
  }

  private loadData(): void {
    const profile = this.profileService.profile();
    if (!profile) {
      this.loading = false;
      return;
    }

    const plansReq = new ListMembershipPlansRequest();
    plansReq.gymId = profile.gymId;
    plansReq.paging.pageSize = 50;

    const ordersReq = new ListOrdersWithItemsRequest();
    ordersReq.paging.pageSize = 100;

    const trainersReq = new ListTrainersRequest();
    trainersReq.gymId = profile.gymId;
    trainersReq.paging.pageSize = 200;

    this.loading = true;

    forkJoin({
      plans: this.plansApi.list(plansReq).pipe(catchError(() => of({ items: [] }))),
      orders: this.ordersApi.listWithItems(ordersReq).pipe(catchError(() => of({ items: [] }))),
      trainers: this.trainersApi.list(trainersReq).pipe(catchError(() => of({ items: [] }))),
    })
      .pipe(
        switchMap(({ plans, orders, trainers }) => {
          const planItems = plans.items ?? [];
          this.membershipPlan =
            planItems.length > 0 ? planItems[Math.min(2, planItems.length - 1)] : null;

          this.purchases = (orders.items ?? []).filter((o) => this.isCurrentUserOrder(o, profile));

          const trainerRows = trainers.items ?? [];
          const trainerIdsAtGym = new Set(trainerRows.map((t) => t.id));
          const trainerUserIds = trainerRows.map((t) => t.userId);

          return forkJoin({
            trainings: this.loadTrainings(profile.id, profile.roleId, trainerIdsAtGym),
            users: trainerUserIds.length
              ? forkJoin(
                  trainerUserIds.map((id) =>
                    this.usersApi.getById(id).pipe(catchError(() => of(null))),
                  ),
                )
              : of([] as ({ firstName: string; lastName: string } | null)[]),
            trainerRows: of(trainerRows),
          });
        }),
      )
      .subscribe({
        next: ({ trainings, users, trainerRows }) => {
          const nameByTrainerId = new Map<number, string>();
          trainerRows.forEach((t, i) => {
            const u = users[i];
            nameByTrainerId.set(
              t.id,
              u ? `${u.firstName} ${u.lastName}`.trim() : `Trener #${t.id}`,
            );
          });

          this.upcomingTrainings = trainings
            .sort((a, b) => a.date.localeCompare(b.date))
            .slice(0, 5)
            .map((t) => ({
              id: t.id,
              trainerName: nameByTrainerId.get(t.trainerId) ?? `Trener #${t.trainerId}`,
              typeLabelKey: this.trainingTypeKey(t.type),
              date: t.date,
            }));

          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }

  private loadTrainings(
    userId: number,
    roleId: number,
    trainerIdsAtGym: Set<number>,
  ) {
    const req = new ListTrainingsRequest();
    req.paging.pageSize = 100;
    req.dateFrom = new Date().toISOString().slice(0, 10);

    if (roleId === TRAINER_ROLE_ID) {
      const trainersReq = new ListTrainersRequest();
      trainersReq.userId = userId;
      trainersReq.paging.pageSize = 1;

      return this.trainersApi.list(trainersReq).pipe(
        switchMap((res) => {
          const trainer = res.items?.[0];
          if (!trainer) return of([] as ListTrainingsQueryDto[]);
          req.trainerId = trainer.id;
          return this.trainingsApi.list(req).pipe(
            map((r) => r.items ?? []),
            catchError(() => of([] as ListTrainingsQueryDto[])),
          );
        }),
        catchError(() => of([] as ListTrainingsQueryDto[])),
      );
    }

    return this.trainingsApi.list(req).pipe(
      map((r) => (r.items ?? []).filter((t) => trainerIdsAtGym.has(t.trainerId))),
      catchError(() => of([] as ListTrainingsQueryDto[])),
    );
  }

  private isCurrentUserOrder(
    order: ListOrdersWithItemsQueryDto,
    profile: { firstName: string; lastName: string },
  ): boolean {
    const fn = order.user?.userFirstname?.trim().toLowerCase();
    const ln = order.user?.userLastname?.trim().toLowerCase();
    return (
      fn === profile.firstName.trim().toLowerCase() &&
      ln === profile.lastName.trim().toLowerCase()
    );
  }
}
