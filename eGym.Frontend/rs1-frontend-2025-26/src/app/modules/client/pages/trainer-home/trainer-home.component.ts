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
  profileService = inject(UserProfileService);

  loading = true;
  clientCount = 0;
  sessionsToday = 0;
  trainings: ListTrainingsQueryDto[] = [];
  pendingRequests: ListTrainerTrainingRequestQueryDto[] = [];
  bookingActionId: number | null = null;

  ngOnInit(): void {
    this.load();
  }

  formatTime(startTime: string): string {
    return (startTime ?? '').slice(0, 5);
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
            return of({
              trainings: { items: [] },
              members: { totalItems: 0 },
              pending: [] as ListTrainerTrainingRequestQueryDto[],
            });
          }
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
          this.trainings = data.trainings?.items ?? [];
          this.pendingRequests = data.pending ?? [];
          this.clientCount = data.members?.totalItems ?? 0;
          const today = new Date().toISOString().slice(0, 10);
          this.sessionsToday = this.trainings.filter((t) => t.date?.startsWith(today)).length;
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
