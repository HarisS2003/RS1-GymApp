import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import { OrdersApiService } from '../../../../api-services/orders/orders-api.service';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { ListTrainingsRequest } from '../../../../api-services/trainings/trainings-api.models';
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
  profileService = inject(UserProfileService);

  activeTab: 'trainers' | 'members' = 'trainers';
  loading = true;

  trainerRows: AdminTrainerRow[] = [];
  members: ListUsersQueryDto[] = [];

  memberCount = 0;
  monthlyRevenue = 0;
  activeTrainings = 0;

  ngOnInit(): void {
    this.profileService.loadProfile().subscribe(() => this.load());
  }

  setTab(tab: 'trainers' | 'members'): void {
    this.activeTab = tab;
  }

  trainerName(row: AdminTrainerRow): string {
    if (row.user) return `${row.user.firstName} ${row.user.lastName}`;
    return `Trener #${row.trainer.id}`;
  }

  deleteTrainer(row: AdminTrainerRow): void {
    if (!confirm('Obrisati trenera?')) return;
    this.trainersApi.delete(row.trainer.id).subscribe({
      next: () => {
        this.toaster.success('Trener obrisan.');
        this.load();
      },
      error: () => this.toaster.error('Brisanje nije uspjelo.'),
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
        this.activeTrainings = trainings.items?.length ?? 0;
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
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
