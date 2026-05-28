import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { forkJoin, of } from 'rxjs';
import { catchError, map, switchMap } from 'rxjs/operators';
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
import { MEMBER_ROLE_ID, TRAINER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { ToasterService } from '../../../../core/services/toaster.service';
import {
  AddTrainerDialogComponent,
  AddTrainerDialogData,
  AddTrainerDialogResult,
} from '../add-trainer-dialog/add-trainer-dialog.component';
import {
  EditTrainerDialogComponent,
  EditTrainerDialogData,
} from '../edit-trainer-dialog/edit-trainer-dialog.component';
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
  private dialog = inject(MatDialog);
  private translate = inject(TranslateService);
  private router = inject(Router);
  profileService = inject(UserProfileService);

  addingTrainers = false;
  activeTab: 'trainers' | 'products' | 'memberships' = 'trainers';
  loading = true;

  trainerRows: AdminTrainerRow[] = [];
  memberCount = 0;
  monthlyRevenue = 0;
  activeTrainings = 0;

  readonly trainerTableColumns = ['id', 'name', 'spec', 'exp', 'status', 'actions'];

  ngOnInit(): void {
    if (this.profileService.profile()) {
      this.load();
      return;
    }
    this.profileService.loadProfile().subscribe(() => this.load());
  }

  setTab(tab: 'trainers' | 'products' | 'memberships'): void {
    if (tab === 'products') {
      this.router.navigate(['/admin/products']);
      return;
    }
    if (tab === 'memberships') {
      this.router.navigate(['/admin/membership-plans']);
      return;
    }
    this.activeTab = tab;
  }

  goMembershipPlans(): void {
    this.router.navigate(['/admin/membership-plans']);
  }

  trainerName(row: AdminTrainerRow): string {
    if (row.user) return `${row.user.firstName} ${row.user.lastName}`;
    return this.translate.instant('CLIENT.PROFILE.TRAINER_FALLBACK', { id: row.trainer.id });
  }

  openAddTrainer(): void {
    const gymId = this.profileService.profile()?.gymId;
    if (!gymId) {
      this.toaster.error(this.translate.instant('ADMIN_DASH.ADD_TRAINER_NO_GYM'));
      return;
    }

    const data: AddTrainerDialogData = {
      gymId,
      existingTrainerUserIds: this.trainerRows.map((r) => r.trainer.userId),
    };

    this.dialog
      .open(AddTrainerDialogComponent, {
        width: '520px',
        maxHeight: '90vh',
        autoFocus: 'first-tabbable',
        data,
      })
      .afterClosed()
      .subscribe((result: AddTrainerDialogResult | undefined) => {
        if (result) this.promoteToTrainer(result, gymId);
      });
  }

  editTrainer(row: AdminTrainerRow): void {
    const data: EditTrainerDialogData = { row };
    this.dialog
      .open(EditTrainerDialogComponent, { width: '420px', data })
      .afterClosed()
      .subscribe((ok) => {
        if (ok) {
          this.toaster.success(this.translate.instant('ADMIN_DASH.EDIT_TRAINER_SUCCESS'));
          this.load();
        }
      });
  }

  degradeTrainer(row: AdminTrainerRow): void {
    const name = this.trainerName(row);
    const msg = this.translate.instant('ADMIN_DASH.DEGRADE_CONFIRM', { name });
    if (!confirm(msg)) return;

    const user = row.user;
    if (!user) {
      this.toaster.error(this.translate.instant('ADMIN_DASH.DEGRADE_ERROR'));
      return;
    }

    this.trainersApi.delete(row.trainer.id).subscribe({
        next: () => {
          this.toaster.success(this.translate.instant('ADMIN_DASH.DEGRADE_SUCCESS', { name }));
          this.load();
        },
        error: (err) => {
          const message =
            err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('ADMIN_DASH.DEGRADE_ERROR');
          this.toaster.error(message);
        },
      });
  }

  private promoteToTrainer(result: AddTrainerDialogResult, gymId: number): void {
    if (this.addingTrainers) return;
    this.addingTrainers = true;

    const u = result.user;

    this.usersApi
      .update(u.id, {
        firstName: u.firstName,
        lastName: u.lastName,
        email: u.email,
        roleId: TRAINER_ROLE_ID,
        gymId: u.gymId,
      })
      .pipe(
        switchMap(() =>
          this.trainersApi.create({
            userId: u.id,
            gymId,
            bio: result.bio,
            experienceYears: result.experienceYears,
          }),
        ),
      )
      .subscribe({
        next: () => {
          this.addingTrainers = false;
          this.toaster.success(this.translate.instant('ADMIN_DASH.ADD_TRAINER_SUCCESS'));
          this.load();
        },
        error: (err) => {
          this.addingTrainers = false;
          const msg =
            err?.error?.detail ??
            err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('ADMIN_DASH.ADD_TRAINER_ERROR');
          this.toaster.error(msg);
          this.load();
        },
      });
  }

  private load(): void {
    const gymId = this.profileService.profile()?.gymId;
    this.loading = true;

    const trainersReq = new ListTrainersRequest();
    trainersReq.paging.pageSize = 500;

    const trainerUsersReq = new ListUsersRequest();
    trainerUsersReq.roleId = TRAINER_ROLE_ID;
    trainerUsersReq.paging.pageSize = 500;
    if (gymId) trainerUsersReq.gymId = gymId;

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
      trainerUsers: this.usersApi
        .list(trainerUsersReq)
        .pipe(catchError(() => of({ items: [] } as any))),
      members: this.usersApi.list(membersReq).pipe(catchError(() => of({ items: [], totalItems: 0 } as any))),
      trainings: this.trainingsApi.list(trainingsReq).pipe(catchError(() => of({ items: [] } as any))),
      orders: this.ordersApi.list(ordersReq).pipe(catchError(() => of({ items: [] } as any))),
    }).subscribe({
      next: ({ trainers, trainerUsers, members, trainings, orders }) => {
        this.memberCount = members.totalItems ?? members.items?.length ?? 0;
        this.activeTrainings = trainings.items?.length ?? 0;
        this.monthlyRevenue = (orders.items ?? []).reduce(
          (sum: number, o: { totalAmount?: number }) => sum + (o.totalAmount ?? 0),
          0,
        );

        const byUserId = new Map<number, ListTrainersQueryDto>();
        for (const t of (trainers.items ?? []) as ListTrainersQueryDto[]) {
          if (!gymId || t.gymId === gymId) {
            byUserId.set(t.userId, t);
          }
        }

        const matched: ListTrainersQueryDto[] = [];
        for (const u of (trainerUsers.items ?? []) as ListUsersQueryDto[]) {
          const t = byUserId.get(u.id);
          if (t) matched.push(t);
        }

        this.mapTrainerRows(matched.length ? matched : [...byUserId.values()]);
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
