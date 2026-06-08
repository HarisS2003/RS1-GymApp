import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ListProductsRequest } from '../../../../api-services/products/products-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListProductsQueryDto } from '../../../../api-services/products/products-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { TranslateService } from '@ngx-translate/core';
import { TrainingsApiService } from '../../../../api-services/trainings/trainings-api.service';
import {
  ListTrainingsQueryDto,
  ListTrainingsRequest,
} from '../../../../api-services/trainings/trainings-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';

@Component({
  selector: 'app-client-home',
  standalone: false,
  templateUrl: './client-home.component.html',
  styleUrl: './client-home.component.scss',
})
export class ClientHomeComponent implements OnInit {
  private usersApi = inject(UsersApiService);
  private trainersApi = inject(TrainersApiService);
  private trainingsApi = inject(TrainingsApiService);
  private productsApi = inject(ProductsApiService);
  profileService = inject(UserProfileService);
  private translate = inject(TranslateService);
  private toaster = inject(ToasterService);

  loading = true;
  memberCount = 0;
  trainerCount = 0;
  groupTrainingCount = 0;
  groupTrainings: ListTrainingsQueryDto[] = [];
  products: ListProductsQueryDto[] = [];
  joiningId: number | null = null;

  ngOnInit(): void {
    if (this.profileService.profile()?.gymId) {
      this.loadData();
      return;
    }
    this.profileService.loadProfile().subscribe({
      next: () => this.loadData(),
      error: () => this.loadData(),
    });
  }

  freePlaces(training: ListTrainingsQueryDto): number {
    return Math.max(training.capacity - training.participantsCount, 0);
  }

  formatTrainingTime(training: ListTrainingsQueryDto): string {
    return `${new Date(training.date).toLocaleDateString('bs-BA')} · ${(training.startTime ?? '').slice(0, 5)}`;
  }

  joinGroupTraining(training: ListTrainingsQueryDto): void {
    if (this.joiningId || this.freePlaces(training) <= 0) return;
    if (this.trainingStartsAt(training) < this.groupJoinCutoff()) {
      this.toaster.error(this.translate.instant('CLIENT.HOME.JOIN_GROUP_TOO_LATE'));
      return;
    }

    this.joiningId = training.id;
    this.trainingsApi.join(training.id).subscribe({
      next: () => {
        this.joiningId = null;
        this.toaster.success(this.translate.instant('CLIENT.HOME.GROUP_JOIN_SUCCESS'));
        this.loadData();
      },
      error: (err) => {
        this.joiningId = null;
        this.toaster.error(
          err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('CLIENT.HOME.GROUP_JOIN_ERROR'),
        );
      },
    });
  }

  private loadData(): void {
    const gymId = this.profileService.profile()?.gymId;

    const usersReq = new ListUsersRequest();
    usersReq.roleId = MEMBER_ROLE_ID;
    usersReq.paging.pageSize = 500;

    const trainersReq = new ListTrainersRequest();
    trainersReq.paging.pageSize = 500;

    const trainingsReq = new ListTrainingsRequest();
    trainingsReq.type = 2;
    trainingsReq.dateFrom = new Date().toISOString().slice(0, 10);
    trainingsReq.paging.pageSize = 50;

    const productsReq = new ListProductsRequest();
    productsReq.gymId = gymId;
    productsReq.paging.pageSize = 50;

    forkJoin({
      members: this.usersApi.list(usersReq).pipe(catchError(() => of({ items: [], totalItems: 0 } as any))),
      trainers: this.trainersApi.list(trainersReq).pipe(catchError(() => of({ items: [], totalItems: 0 } as any))),
      trainings: this.trainingsApi.list(trainingsReq).pipe(catchError(() => of({ items: [] } as any))),
      products: this.productsApi.list(productsReq).pipe(catchError(() => of({ items: [] } as any))),
    }).subscribe({
      next: ({ members, trainers, trainings, products }) => {
        this.memberCount = members.totalItems ?? members.items?.length ?? 0;
        this.trainerCount = trainers.totalItems ?? trainers.items?.length ?? 0;
        const joinCutoff = this.groupJoinCutoff();
        this.groupTrainings = (trainings.items ?? [])
          .filter((t: ListTrainingsQueryDto) => this.trainingStartsAt(t) >= joinCutoff)
          .sort(
            (a: ListTrainingsQueryDto, b: ListTrainingsQueryDto) =>
              this.trainingStartsAt(a).getTime() - this.trainingStartsAt(b).getTime(),
          );
        this.groupTrainingCount = this.groupTrainings.length;
        this.products = (products.items ?? []).slice(0, 4);
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

  private groupJoinCutoff(): Date {
    return new Date(Date.now() + 60 * 60 * 1000);
  }
}
