import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { ListTrainingsRequest } from '../../../../api-services/trainings/trainings-api.models';
import { ListTrainingsQueryDto } from '../../../../api-services/trainings/trainings-api.models';
import { ListOrdersRequest } from '../../../../api-services/orders/orders-api.models';
import { ListTrainersQueryDto } from '../../../../api-services/trainers/trainers-api.models';
import { ListUsersQueryDto } from '../../../../api-services/users/users-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { ToasterService } from '../../../../core/services/toaster.service';

export interface AdminTrainerRow {
  trainer: ListTrainersQueryDto;
  user?: ListUsersQueryDto;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: false,
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss',
})
export class AdminDashboardComponent implements OnInit {
  private trainersApi = inject(TrainersApiService);
  private usersApi = inject(UsersApiService);
  private trainingsApi = inject(TrainingsApiService);
  private ordersApi = inject(OrdersApiService);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private snackBar = inject(MatSnackBar);
  profileService = inject(UserProfileService);

  activeTab: 'trainers' | 'trainings' | 'members' = 'trainers';
  loading = true;

  trainerRows: AdminTrainerRow[] = [];
  trainingRows: ListTrainingsQueryDto[] = [];
  members: ListUsersQueryDto[] = [];
  trainerNameMap = new Map<number, string>();

  memberCount = 0;
  monthlyRevenue = 0;
  activeTrainings = 0;
  private pendingTrainings: ListTrainingsQueryDto[] = [];

  ngOnInit(): void {
    this.profileService.loadProfile().subscribe(() => this.load());
  }

  setTab(tab: 'trainers' | 'trainings' | 'members'): void {
    this.activeTab = tab;
  }

  trainerName(row: AdminTrainerRow): string {
    if (row.user) return `${row.user.firstName} ${row.user.lastName}`;
    return `Trener #${row.trainer.id}`;
  }

  trainerNameById(trainerId: number): string {
    return this.trainerNameMap.get(trainerId) ?? `Trener #${trainerId}`;
  }

  trainingTypeLabel(type: number): string {
    return type === 2
      ? this.translate.instant('ADMIN_DASH.TYPE_GROUP')
      : this.translate.instant('ADMIN_DASH.TYPE_INDIVIDUAL');
  }

  onAddTrainer(): void {
    this.snackBar.open(this.translate.instant('ADMIN_DASH.ADD_UNAVAILABLE'), undefined, {
      duration: 3500,
    });
  }

  onEditTrainer(_row: AdminTrainerRow): void {
    this.snackBar.open(this.translate.instant('ADMIN_DASH.EDIT_UNAVAILABLE'), undefined, {
      duration: 3500,
    });
  }

  deleteTrainer(row: AdminTrainerRow): void {
    if (!confirm(this.translate.instant('ADMIN_DASH.DELETE_CONFIRM'))) return;
    this.trainersApi.delete(row.trainer.id).subscribe({
      next: () => {
        this.toaster.success(this.translate.instant('ADMIN_DASH.DELETE_SUCCESS'));
        this.load();
      },
      error: () => this.toaster.error(this.translate.instant('ADMIN_DASH.DELETE_ERROR')),
    });
  }

  private load(): void {
    const gymId = this.profileService.profile()?.gymId;

    const trainersReq = new ListTrainersRequest();
    trainersReq.paging.pageSize = 200;
    if (gymId) trainersReq.gymId = gymId;

    const membersReq = new ListUsersRequest();
    membersReq.roleId = MEMBER_ROLE_ID;
    membersReq.paging.pageSize = 500;
    if (gymId) membersReq.gymId = gymId;

    const trainingsReq = new ListTrainingsRequest();
    trainingsReq.paging.pageSize = 500;

    const ordersReq = new ListOrdersRequest();
    ordersReq.paging.pageSize = 500;

    forkJoin({
      trainers: this.trainersApi.list(trainersReq).pipe(catchError(() => of({ items: [] } as any))),
      members: this.usersApi.list(membersReq).pipe(catchError(() => of({ items: [], totalItems: 0 } as any))),
      trainings: this.trainingsApi.list(trainingsReq).pipe(catchError(() => of({ items: [] } as any))),
      orders: this.ordersApi.list(ordersReq).pipe(catchError(() => of({ items: [] } as any))),
    }).subscribe({
      next: ({ trainers, members, trainings, orders }) => {
        this.members = members.items ?? [];
        this.memberCount = members.totalItems ?? this.members.length;
        this.pendingTrainings = trainings.items ?? [];
        this.activeTrainings = this.pendingTrainings.length;
        this.monthlyRevenue = (orders.items ?? []).reduce(
          (sum: number, o: { totalAmount?: number }) => sum + (o.totalAmount ?? 0),
          0,
        );
        this.mapTrainerRows(trainers.items ?? []);
      },
      error: () => (this.loading = false),
    });
  }

  private mapTrainerRows(list: ListTrainersQueryDto[]): void {
    if (!list.length) {
      this.trainerRows = [];
      this.trainerNameMap.clear();
      this.loading = false;
      return;
    }

    forkJoin(
      list.map((t) =>
        this.usersApi.getById(t.userId).pipe(
          catchError(() => of(null)),
          map((user): AdminTrainerRow => ({
            trainer: t,
            user: user ?? undefined,
          })),
        ),
      ),
    ).subscribe({
      next: (rows) => {
        this.trainerRows = rows as AdminTrainerRow[];
        this.trainerNameMap.clear();
        this.trainerRows.forEach((r) => {
          this.trainerNameMap.set(r.trainer.id, this.trainerName(r));
        });
        const gymTrainerIds = new Set(this.trainerRows.map((r) => r.trainer.id));
        this.trainingRows =
          gymTrainerIds.size > 0
            ? this.pendingTrainings.filter((t) => gymTrainerIds.has(t.trainerId))
            : this.pendingTrainings;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
