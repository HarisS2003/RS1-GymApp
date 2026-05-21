import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, switchMap } from 'rxjs/operators';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListTrainingsQueryDto, ListTrainingsRequest } from '../../../../api-services/trainings/trainings-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';

export interface ScheduleSlotView {
  training: ListTrainingsQueryDto;
  title: string;
  subtitle: string;
  isGroup: boolean;
}

export interface GroupOccupancyView {
  id: number;
  label: string;
  time: string;
  filled: number;
  capacity: number;
  percent: number;
  isFull: boolean;
}

@Component({
  selector: 'app-client-schedule',
  standalone: false,
  templateUrl: './client-schedule.component.html',
  styleUrl: './client-schedule.component.scss',
})
export class ClientScheduleComponent implements OnInit {
  profileService = inject(UserProfileService);
  private trainersApi = inject(TrainersApiService);
  private trainingsApi = inject(TrainingsApiService);
  private usersApi = inject(UsersApiService);
  private translate = inject(TranslateService);
  private snackBar = inject(MatSnackBar);

  loading = true;
  todayLabel = '';
  totalTrainings = 0;
  individualCount = 0;
  groupCount = 0;
  clientCount = 0;
  pointsTotal = 0;
  todaySlots: ScheduleSlotView[] = [];
  groupOccupancy: GroupOccupancyView[] = [];

  ngOnInit(): void {
    this.todayLabel = this.formatTodayLabel();
    const profile = this.profileService.profile();
    if (!profile) {
      this.profileService.loadProfile().subscribe(() => this.load());
      return;
    }
    this.load();
  }

  onAddAppointment(): void {
    this.snackBar.open(this.translate.instant('CLIENT.SCHEDULE.ADD_UNAVAILABLE'), undefined, {
      duration: 3500,
    });
  }

  onCreateGroup(): void {
    this.snackBar.open(this.translate.instant('CLIENT.SCHEDULE.CREATE_UNAVAILABLE'), undefined, {
      duration: 3500,
    });
  }

  private load(): void {
    const profile = this.profileService.profile();
    if (!profile) {
      this.loading = false;
      return;
    }

    const trainersReq = new ListTrainersRequest();
    trainersReq.userId = profile.id;
    trainersReq.paging.pageSize = 1;

    const membersReq = new ListUsersRequest();
    membersReq.gymId = profile.gymId;
    membersReq.roleId = MEMBER_ROLE_ID;
    membersReq.paging.pageSize = 500;

    this.loading = true;

    this.trainersApi
      .list(trainersReq)
      .pipe(
        switchMap((res) => {
          const trainer = res.items?.[0];
          if (!trainer) {
            return of({ trainings: [] as ListTrainingsQueryDto[], members: { totalItems: 0 } });
          }
          const tReq = new ListTrainingsRequest();
          tReq.trainerId = trainer.id;
          tReq.paging.pageSize = 500;
          return forkJoin({
            trainings: this.trainingsApi.list(tReq).pipe(
              mapTrainings(),
              catchError(() => of([] as ListTrainingsQueryDto[])),
            ),
            members: this.usersApi.list(membersReq).pipe(catchError(() => of({ totalItems: 0 }))),
          });
        }),
        catchError(() => of({ trainings: [] as ListTrainingsQueryDto[], members: { totalItems: 0 } })),
      )
      .subscribe({
        next: ({ trainings, members }) => {
          this.applyTrainings(trainings);
          this.clientCount = members.totalItems ?? 0;
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }

  private applyTrainings(trainings: ListTrainingsQueryDto[]): void {
    this.totalTrainings = trainings.length;
    this.individualCount = trainings.filter((t) => t.type === 1).length;
    this.groupCount = trainings.filter((t) => t.type === 2).length;
    this.pointsTotal = trainings.reduce((sum, t) => sum + (t.participantsCount ?? 0), 0);

    const today = new Date().toISOString().slice(0, 10);
    const todayList = trainings
      .filter((t) => t.date?.startsWith(today))
      .sort((a, b) => (a.startTime ?? '').localeCompare(b.startTime ?? ''));

    this.todaySlots = todayList.map((t) => this.toSlot(t));

    this.groupOccupancy = todayList
      .filter((t) => t.type === 2)
      .map((t) => {
        const filled = t.participantsCount ?? 0;
        const capacity = t.capacity || 1;
        const percent = Math.min(100, Math.round((filled / capacity) * 100));
        return {
          id: t.id,
          label: this.groupTitle(t),
          time: this.formatTime(t.startTime),
          filled,
          capacity,
          percent,
          isFull: filled >= capacity,
        };
      });
  }

  private toSlot(t: ListTrainingsQueryDto): ScheduleSlotView {
    const isGroup = t.type === 2;
    return {
      training: t,
      isGroup,
      title: isGroup ? this.groupTitle(t) : this.translate.instant('CLIENT.SCHEDULE.INDIVIDUAL_SESSION'),
      subtitle: isGroup
        ? this.translate.instant('CLIENT.SCHEDULE.TYPE_GROUP')
        : this.participantLabel(t.participantsCount),
    };
  }

  private groupTitle(t: ListTrainingsQueryDto): string {
    return `${this.translate.instant('CLIENT.SCHEDULE.GROUP_TRAINING')} — ${this.translate.instant('CLIENT.SCHEDULE.TYPE_GROUP')}`;
  }

  private participantLabel(count: number): string {
    return this.translate.instant('CLIENT.SCHEDULE.PARTICIPANTS_COUNT', { count });
  }

  formatTime(startTime: string): string {
    if (!startTime) return '—';
    const parts = startTime.split(':');
    if (parts.length >= 2) return `${parts[0]}:${parts[1]}`;
    return startTime;
  }

  private formatTodayLabel(): string {
    return new Date().toLocaleDateString(undefined, {
      weekday: 'long',
      day: 'numeric',
      month: 'short',
      year: 'numeric',
    });
  }
}

function mapTrainings() {
  return switchMap((res: { items?: ListTrainingsQueryDto[] }) => of(res.items ?? []));
}
