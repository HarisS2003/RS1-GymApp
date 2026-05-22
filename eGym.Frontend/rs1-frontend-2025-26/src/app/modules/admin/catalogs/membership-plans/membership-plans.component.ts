import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import {
  ListMembershipPlansQueryDto,
  ListMembershipPlansRequest,
} from '../../../../api-services/membership-plans/membership-plans-api.models';
import { MembershipPlansApiService } from '../../../../api-services/membership-plans/membership-plans-api.service';
import { BaseListPagedComponent } from '../../../../core/components/base-classes/base-list-paged-component';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import { DialogButton } from '../../../shared/models/dialog-config.model';

@Component({
  selector: 'app-membership-plans',
  standalone: false,
  templateUrl: './membership-plans.component.html',
  styleUrl: './membership-plans.component.scss',
})
export class MembershipPlansComponent
  extends BaseListPagedComponent<ListMembershipPlansQueryDto, ListMembershipPlansRequest>
  implements OnInit
{
  private api = inject(MembershipPlansApiService);
  private router = inject(Router);
  private dialogHelper = inject(DialogHelperService);
  private profileService = inject(UserProfileService);

  totalPlans = 0;
  avgPrice = 0;
  withDiscount = 0;

  constructor() {
    super();
    this.request = new ListMembershipPlansRequest();
  }

  ngOnInit(): void {
    const gymId = this.profileService.profile()?.gymId;
    if (gymId) this.request.gymId = gymId;
    this.initList();
    this.loadStats();
  }

  goTrainers(): void {
    this.router.navigate(['/admin/dashboard']);
  }

  goProducts(): void {
    this.router.navigate(['/admin/products']);
  }

  protected loadPagedData(): void {
    this.startLoading();
    this.api.list(this.request).subscribe({
      next: (response) => {
        this.handlePageResult(response);
        this.stopLoading();
      },
      error: () => this.stopLoading('Failed to load membership plans'),
    });
  }

  private loadStats(): void {
    const statsReq = new ListMembershipPlansRequest();
    statsReq.paging.page = 1;
    statsReq.paging.pageSize = 500;
    const gymId = this.profileService.profile()?.gymId;
    if (gymId) statsReq.gymId = gymId;

    this.api.list(statsReq).subscribe({
      next: (res) => {
        const all = res.items ?? [];
        this.totalPlans = res.totalItems ?? all.length;
        this.withDiscount = all.filter((p) => p.discountPercentage > 0).length;
        this.avgPrice =
          all.length > 0 ? all.reduce((s, p) => s + p.price, 0) / all.length : 0;
      },
    });
  }

  onCreate(): void {
    this.router.navigate(['/admin/membership-plans/add']);
  }

  onEdit(plan: ListMembershipPlansQueryDto): void {
    this.router.navigate(['/admin/membership-plans', plan.id, 'edit']);
  }

  onDelete(plan: ListMembershipPlansQueryDto): void {
    this.dialogHelper.membershipPlan.confirmDelete(plan.name).subscribe((result) => {
      if (result?.button === DialogButton.DELETE) this.performDelete(plan);
    });
  }

  private performDelete(plan: ListMembershipPlansQueryDto): void {
    this.startLoading();
    this.api.delete(plan.id).subscribe({
      next: () => {
        this.dialogHelper.membershipPlan.showDeleteSuccess().subscribe();
        this.loadPagedData();
        this.loadStats();
      },
      error: () => {
        this.stopLoading();
        this.dialogHelper.showError(
          'DIALOGS.TITLES.ERROR',
          'MEMBERSHIP_PLANS.DIALOGS.ERROR_DELETE',
        ).subscribe();
      },
    });
  }

  onSearch(): void {
    this.request.paging.page = 1;
    this.loadPagedData();
  }
}
