import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { TrainingRequestsApiService } from '../../../../api-services/training-requests/training-requests-api.service';
import { ListTrainingsRequest } from '../../../../api-services/trainings/trainings-api.models';
import { ListTrainingsQueryDto } from '../../../../api-services/trainings/trainings-api.models';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import {
  ListTrainerTrainingRequestQueryDto,
  TRAINING_REQUEST_PENDING,
} from '../../../../api-services/training-requests/training-requests-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { TranslateService } from '@ngx-translate/core';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MatDialog } from '@angular/material/dialog';
import {
  GroupTrainingDialogComponent,
  GroupTrainingDialogResult,
} from './group-training-dialog.component';

@Component({
  selector: 'app-trainer-home',
  standalone: false,
  templateUrl: './trainer-home.component.html',
  styleUrl: './trainer-home.component.scss',
})
export class TrainerHomeComponent implements OnInit {
  private trainersApi = inject(TrainersApiService);
  private trainingsApi = inject(TrainingsApiService);
  private usersApi = inject(UsersApiService);
  private requestsApi = inject(TrainingRequestsApiService);
  private translate = inject(TranslateService);
  private toaster = inject(ToasterService);
  private dialog = inject(MatDialog);
  profileService = inject(UserProfileService);

  loading = true;
  clientCount = 0;
  sessionsToday = 0;
  trainings: ListTrainingsQueryDto[] = [];
  todayTrainings: ListTrainingsQueryDto[] = [];
  upcomingGroupTrainings: ListTrainingsQueryDto[] = [];
  individualCount = 0;
  groupCount = 0;
  pendingRequests: ListTrainerTrainingRequestQueryDto[] = [];
  bookingActionId: number | null = null;
  private trainerId: number | null = null;

  ngOnInit(): void {
    this.load();
  }

  formatTime(startTime: string): string {
    return (startTime ?? '').slice(0, 5);
  }

  typeLabel(type: number): string {
    if (type === 2) return this.translate.instant('CLIENT.TRAINER_HOME.TYPE_GROUP');
    return this.translate.instant('CLIENT.TRAINER_HOME.TYPE_INDIVIDUAL');
  }

  groupProgress(training: ListTrainingsQueryDto): number {
    if (!training.capacity) return 0;
    return Math.min((training.participantsCount / training.capacity) * 100, 100);
  }

  approve(id: number): void {
    if (this.bookingActionId != null) return;
    this.bookingActionId = id;
    this.requestsApi.approve(id).subscribe({
      next: () => {
        this.bookingActionId = null;
        this.toaster.success(this.translate.instant('CLIENT.PROFILE.BOOKING_ACCEPTED'));
        this.load();
      },
      error: (err) => {
        this.bookingActionId = null;
        this.toaster.error(err?.error?.message ?? 'Error');
      },
    });
  }

  reject(id: number): void {
    if (this.bookingActionId != null) return;
    this.bookingActionId = id;
    this.requestsApi.reject(id).subscribe({
      next: () => {
        this.bookingActionId = null;
        this.toaster.success(this.translate.instant('CLIENT.PROFILE.BOOKING_DECLINED'));
        this.load();
      },
      error: () => {
        this.bookingActionId = null;
      },
    });
  }

  openGroupTrainingDialog(): void {
    if (!this.trainerId) return;

    this.dialog
      .open<GroupTrainingDialogComponent, void, GroupTrainingDialogResult>(
        GroupTrainingDialogComponent,
        { width: '560px' },
      )
      .afterClosed()
      .subscribe((result) => {
        if (!result || !this.trainerId) return;

        this.trainingsApi
          .create({
            trainerId: this.trainerId,
            type: 2,
            description: result.description,
            date: result.date,
            startTime: result.startTime,
            capacity: result.capacity,
          })
          .subscribe({
            next: () => {
              this.toaster.success(this.translate.instant('CLIENT.TRAINER_HOME.GROUP_CREATED'));
              this.load();
            },
            error: (err) =>
              this.toaster.error(
                err?.error?.message ??
                  err?.error?.title ??
                  this.translate.instant('CLIENT.TRAINER_HOME.GROUP_CREATE_ERROR'),
              ),
          });
      });
  }

  private load(): void {
    const profile = this.profileService.profile();
    if (!profile) {
      this.profileService.loadProfile().subscribe(() => this.load());
      return;
    }

    const trainersReq = new ListTrainersRequest();
    trainersReq.userId = profile.id;
    trainersReq.gymId = profile.gymId;
    trainersReq.paging.pageSize = 1;

    const membersReq = new ListUsersRequest();
    membersReq.gymId = profile.gymId;
    membersReq.roleId = MEMBER_ROLE_ID;
    membersReq.paging.pageSize = 500;

    this.trainersApi
      .list(trainersReq)
      .pipe(
        switchMap((res) => {
          const trainer = res.items?.[0];
          if (!trainer) {
            this.trainerId = null;
            return of({
              trainings: { items: [] },
              members: { totalItems: 0 },
              pending: [] as ListTrainerTrainingRequestQueryDto[],
            });
          }
          this.trainerId = trainer.id;
          const tReq = new ListTrainingsRequest();
          tReq.trainerId = trainer.id;
          tReq.paging.pageSize = 50;
          return forkJoin({
            trainings: this.trainingsApi.list(tReq).pipe(catchError(() => of({ items: [] } as any))),
            members: this.usersApi.list(membersReq).pipe(catchError(() => of({ totalItems: 0 } as any))),
            pending: this.requestsApi
              .listForTrainer(TRAINING_REQUEST_PENDING)
              .pipe(catchError(() => of([]))),
          });
        }),
        catchError(() =>
          of({ trainings: { items: [] }, members: { totalItems: 0 }, pending: [] }),
        ),
      )
      .subscribe({
        next: (data: any) => {
          const allTrainings = (data.trainings?.items ?? []) as ListTrainingsQueryDto[];
          const now = new Date();
          this.trainings = allTrainings
            .filter((t) => this.trainingStartsAt(t) >= now)
            .sort((a, b) => this.trainingStartsAt(a).getTime() - this.trainingStartsAt(b).getTime());
          this.individualCount = allTrainings.filter((t) => t.type === 1).length;
          this.groupCount = allTrainings.filter((t) => t.type === 2).length;
          this.pendingRequests = data.pending ?? [];
          this.clientCount = data.members?.totalItems ?? 0;
          const today = new Date().toISOString().slice(0, 10);
          this.todayTrainings = this.trainings.filter((t) => t.date?.startsWith(today));
          this.upcomingGroupTrainings = this.trainings.filter((t) => t.type === 2).slice(0, 3);
          this.sessionsToday = this.todayTrainings.length;
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }

  private trainingStartsAt(training: ListTrainingsQueryDto): Date {
    const date = (training.date ?? '').slice(0, 10);
    const time = (training.startTime ?? '00:00:00').slice(0, 8);
    return new Date(`${date}T${time}`);
  }
}
