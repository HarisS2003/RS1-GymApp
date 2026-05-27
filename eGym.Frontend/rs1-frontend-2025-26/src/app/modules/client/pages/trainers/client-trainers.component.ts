import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListTrainersQueryDto } from '../../../../api-services/trainers/trainers-api.models';
import {
  GetUserByIdQueryDto,
  ListUsersQueryDto,
} from '../../../../api-services/users/users-api.models';
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

  bookTrainer(trainerId: number): void {
    if (!this.hasActiveMembership) {
      this.toaster.error(this.translate.instant('CLIENT.TRAINER_BOOKING.MEMBERSHIP_REQUIRED'));
      return;
    }
    this.router.navigate(['/client/trainers', trainerId, 'book']);
  }

  displayName(row: TrainerRow): string {
    if (row.user) return `${row.user.firstName} ${row.user.lastName}`;
    return `Trener #${row.trainer.id}`;
  }

  private load(): void {
    const gymId = this.profileService.profile()?.gymId;
    if (!gymId) {
      this.profileService.loadProfile().subscribe(() => this.load());
      return;
    }

    const req = new ListTrainersRequest();
    req.gymId = gymId;
    req.paging.pageSize = 100;

    this.trainersApi
      .list(req)
      .pipe(catchError(() => of({ items: [] } as any)))
      .subscribe({
        next: (res) => {
          const trainers = res.items ?? [];
          if (!trainers.length) {
            this.rows = [];
            this.loading = false;
            return;
          }
          forkJoin(
            trainers.map((t: ListTrainersQueryDto) =>
              this.usersApi.getById(t.userId).pipe(
                catchError(() => of(null)),
                map((user: GetUserByIdQueryDto | null): TrainerRow => ({
                  trainer: t,
                  user: user ?? undefined,
                })),
              ),
            ),
          ).subscribe({
            next: (rows) => {
              this.rows = rows as TrainerRow[];
              this.loading = false;
            },
            error: () => (this.loading = false),
          });
        },
        error: () => (this.loading = false),
      });
  }
}
