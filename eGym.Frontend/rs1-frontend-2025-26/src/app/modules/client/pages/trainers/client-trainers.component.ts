import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListTrainersQueryDto } from '../../../../api-services/trainers/trainers-api.models';
import { ListUsersRequest, ListUsersQueryDto } from '../../../../api-services/users/users-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { UserMembershipsApiService } from '../../../../api-services/user-memberships/user-memberships-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';

export interface TrainerRow {
  trainer: ListTrainersQueryDto;
  user?: ListUsersQueryDto;
}

@Component({
  selector: 'app-client-trainers',
  standalone: false,
  templateUrl: './client-trainers.component.html',
  styleUrl: './client-trainers.component.scss',
})
export class ClientTrainersComponent implements OnInit {
  private trainersApi = inject(TrainersApiService);
  private usersApi = inject(UsersApiService);
  private membershipsApi = inject(UserMembershipsApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toaster = inject(ToasterService);
  profileService = inject(UserProfileService);

  loading = true;
  rows: TrainerRow[] = [];
  hasActiveMembership = false;

  ngOnInit(): void {
    this.membershipsApi.getMyActive().subscribe({
      next: (active) => (this.hasActiveMembership = !!active),
      error: () => (this.hasActiveMembership = false),
    });
    this.load();
  }

  bookTrainer(trainerPublicId: string): void {
    if (!this.hasActiveMembership) {
      this.toaster.error(this.translate.instant('CLIENT.TRAINER_BOOKING.MEMBERSHIP_REQUIRED'));
      return;
    }
    this.router.navigate(['/client/trainers', trainerPublicId, 'book']);
  }

  displayName(row: TrainerRow): string {
    if (row.user) return `${row.user.firstName} ${row.user.lastName}`;
    return `Trener ${row.trainer.publicId.slice(0, 8)}…`;
  }

  private load(): void {
    const trainersReq = new ListTrainersRequest();
    trainersReq.paging.pageSize = 100;

    const usersReq = new ListUsersRequest();
    usersReq.paging.pageSize = 500;

    forkJoin({
      trainers: this.trainersApi.list(trainersReq).pipe(catchError(() => of({ items: [] }))),
      users: this.usersApi.list(usersReq).pipe(catchError(() => of({ items: [] }))),
    }).subscribe({
      next: ({ trainers, users }) => {
        const usersByPublicId = new Map(
          (users.items ?? []).map((u: ListUsersQueryDto) => [u.publicId, u]),
        );
        this.rows = (trainers.items ?? []).map((trainer: ListTrainersQueryDto) => ({
          trainer,
          user: usersByPublicId.get(trainer.userPublicId),
        }));
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
