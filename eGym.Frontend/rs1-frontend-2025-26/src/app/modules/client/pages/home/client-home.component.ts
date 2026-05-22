import { Component, OnInit, inject } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { MembershipPlansApiService } from '../../../../api-services/membership-plans/membership-plans-api.service';
import { ProductsApiService } from '../../../../api-services/products/products-api.service';
import { ListMembershipPlansRequest } from '../../../../api-services/membership-plans/membership-plans-api.models';
import { ListProductsRequest } from '../../../../api-services/products/products-api.models';
import { ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListMembershipPlansQueryDto } from '../../../../api-services/membership-plans/membership-plans-api.models';
import { ListProductsQueryDto } from '../../../../api-services/products/products-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-client-home',
  standalone: false,
  templateUrl: './client-home.component.html',
  styleUrl: './client-home.component.scss',
})
export class ClientHomeComponent implements OnInit {
  private usersApi = inject(UsersApiService);
  private trainersApi = inject(TrainersApiService);
  private plansApi = inject(MembershipPlansApiService);
  private productsApi = inject(ProductsApiService);
  profileService = inject(UserProfileService);
  private translate = inject(TranslateService);

  loading = true;
  memberCount = 0;
  trainerCount = 0;
  planCount = 0;
  plans: ListMembershipPlansQueryDto[] = [];
  products: ListProductsQueryDto[] = [];

  ngOnInit(): void {
    const gymId = this.profileService.profile()?.gymId;
    if (!gymId) {
      this.profileService.loadProfile().subscribe(() => this.loadData());
      return;
    }
    this.loadData();
  }

  formatDuration(days: number): string {
    return `${days} ${this.translate.instant('COMMON.DAYS')}`;
  }

  finalPrice(plan: ListMembershipPlansQueryDto): number {
    const discount = (plan.price * plan.discountPercentage) / 100;
    return Math.round((plan.price - discount) * 100) / 100;
  }

  isRecommended(plan: ListMembershipPlansQueryDto): boolean {
    return this.plans.length > 0 && plan.id === this.plans[Math.min(2, this.plans.length - 1)]?.id;
  }

  private loadData(): void {
    const gymId = this.profileService.profile()?.gymId;
    if (!gymId) {
      this.loading = false;
      return;
    }

    const usersReq = new ListUsersRequest();
    usersReq.gymId = gymId;
    usersReq.roleId = MEMBER_ROLE_ID;
    usersReq.paging.pageSize = 500;

    const trainersReq = new ListTrainersRequest();
    trainersReq.gymId = gymId;
    trainersReq.paging.pageSize = 500;

    const plansReq = new ListMembershipPlansRequest();
    plansReq.gymId = gymId;
    plansReq.paging.pageSize = 50;

    const productsReq = new ListProductsRequest();
    productsReq.gymId = gymId;
    productsReq.paging.pageSize = 50;

    forkJoin({
      members: this.usersApi.list(usersReq).pipe(catchError(() => of({ items: [], totalItems: 0 } as any))),
      trainers: this.trainersApi.list(trainersReq).pipe(catchError(() => of({ items: [], totalItems: 0 } as any))),
      plans: this.plansApi.list(plansReq).pipe(catchError(() => of({ items: [] } as any))),
      products: this.productsApi.list(productsReq).pipe(catchError(() => of({ items: [] } as any))),
    }).subscribe({
      next: ({ members, trainers, plans, products }) => {
        this.memberCount = members.totalItems ?? members.items?.length ?? 0;
        this.trainerCount = trainers.totalItems ?? trainers.items?.length ?? 0;
        this.plans = plans.items ?? [];
        this.planCount = this.plans.length;
        this.products = (products.items ?? []).slice(0, 4);
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
