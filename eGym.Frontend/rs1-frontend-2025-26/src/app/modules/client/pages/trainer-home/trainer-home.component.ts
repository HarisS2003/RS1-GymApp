import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ListTrainingsRequest } from '../../../../api-services/trainings/trainings-api.models';
import { ListTrainingsQueryDto } from '../../../../api-services/trainings/trainings-api.models';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';

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
  profileService = inject(UserProfileService);

  loading = true;
  clientCount = 0;
  sessionsToday = 0;
  trainings: ListTrainingsQueryDto[] = [];

  ngOnInit(): void {
    this.load();
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
            return of({ trainings: { items: [] }, members: { totalItems: 0 } });
          }
          const tReq = new ListTrainingsRequest();
          tReq.trainerId = trainer.id;
          tReq.paging.pageSize = 50;
          return forkJoin({
            trainings: this.trainingsApi.list(tReq).pipe(catchError(() => of({ items: [] } as any))),
            members: this.usersApi.list(membersReq).pipe(catchError(() => of({ totalItems: 0 } as any))),
          });
        }),
        catchError(() =>
          of({ trainings: { items: [] }, members: { totalItems: 0 } }),
        ),
      )
      .subscribe({
        next: (data: any) => {
          this.trainings = data.trainings?.items ?? [];
          this.clientCount = data.members?.totalItems ?? 0;
          const today = new Date().toISOString().slice(0, 10);
          this.sessionsToday = this.trainings.filter((t) => t.date?.startsWith(today)).length;
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
