import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { catchError, of } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { MembershipPlansApiService } from '../../../../api-services/membership-plans/membership-plans-api.service';
import {
  ListMembershipPlansQueryDto,
  ListMembershipPlansRequest,
} from '../../../../api-services/membership-plans/membership-plans-api.models';
import {
  PAYMENT_METHOD_CASH,
} from '../../../../api-services/user-memberships/user-memberships-api.models';
import { UserMembershipsApiService } from '../../../../api-services/user-memberships/user-memberships-api.service';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { DialogHelperService } from '../../../shared/services/dialog-helper.service';
import {
  MembershipPurchaseDialogComponent,
  MembershipPurchaseDialogData,
  MembershipPurchaseDialogResult,
} from '../../../shared/components/membership-purchase-dialog/membership-purchase-dialog.component';
import {
  MembershipAlreadyHaveDialogComponent,
  MembershipAlreadyHaveDialogData,
} from '../../../shared/components/membership-already-have-dialog/membership-already-have-dialog.component';
import { GetMyActiveUserMembershipQueryDto } from '../../../../api-services/user-memberships/user-memberships-api.models';

@Component({
  selector: 'app-client-memberships',
  standalone: false,
  templateUrl: './client-memberships.component.html',
  styleUrl: './client-memberships.component.scss',
})
export class ClientMembershipsComponent implements OnInit {
  private plansApi = inject(MembershipPlansApiService);
  private membershipsApi = inject(UserMembershipsApiService);
  private profileService = inject(UserProfileService);
  private toaster = inject(ToasterService);
  private dialogHelper = inject(DialogHelperService);
  private dialog = inject(MatDialog);
  private translate = inject(TranslateService);

  loading = true;
  purchasingId: number | null = null;
  plans: ListMembershipPlansQueryDto[] = [];

  ngOnInit(): void {
    const load = () => this.loadPlans();
    if (this.profileService.profile()) {
      load();
    } else {
      this.profileService.loadProfile().subscribe(() => load());
    }
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

  buyPlan(plan: ListMembershipPlansQueryDto): void {
    if (this.purchasingId !== null) return;

    this.membershipsApi.getMyActive().subscribe({
      next: (active) => {
        if (active) {
          if (plan.durationDays <= active.durationDays) {
            this.showAlreadyHaveDialog(active, plan.durationDays, true);
            return;
          }
          this.openPurchaseDialog(plan, active);
          return;
        }
        this.openPurchaseDialog(plan, null);
      },
      error: () => this.openPurchaseDialog(plan, null),
    });
  }

  private showAlreadyHaveDialog(
    active: GetMyActiveUserMembershipQueryDto,
    selectedDurationDays: number,
    isUpgradeBlocked: boolean,
  ): void {
    const data: MembershipAlreadyHaveDialogData = {
      active,
      selectedDurationDays,
      isUpgradeBlocked,
    };
    this.dialog.open(MembershipAlreadyHaveDialogComponent, {
      width: '420px',
      maxWidth: '95vw',
      panelClass: 'membership-purchase-panel',
      data,
    });
  }

  private openPurchaseDialog(
    plan: ListMembershipPlansQueryDto,
    active: GetMyActiveUserMembershipQueryDto | null,
  ): void {
    const data: MembershipPurchaseDialogData = {
      plan,
      finalPrice: this.finalPrice(plan),
      durationLabel: this.formatDuration(plan.durationDays),
      isUpgrade: !!active,
      currentPlanName: active?.planName,
      currentDurationDays: active?.durationDays,
    };

    this.dialog
      .open(MembershipPurchaseDialogComponent, {
        width: '460px',
        maxWidth: '95vw',
        panelClass: 'membership-purchase-panel',
        autoFocus: false,
        data,
      })
      .afterClosed()
      .subscribe((result: MembershipPurchaseDialogResult | undefined) => {
        if (!result?.confirmed) return;
        this.executePurchase(plan);
      });
  }

  private executePurchase(plan: ListMembershipPlansQueryDto): void {
    this.purchasingId = plan.id;

    this.membershipsApi
      .purchase({
        membershipPlanId: plan.id,
        paymentMethod: PAYMENT_METHOD_CASH,
      })
      .subscribe({
        next: () => {
          this.purchasingId = null;
          this.dialogHelper.membershipPurchase.showSuccess(plan.name).subscribe();
          this.profileService.loadProfile().subscribe();
        },
        error: (err) => {
          this.purchasingId = null;
          const msg =
            err?.error?.detail ??
            err?.error?.message ??
            this.translate.instant('CLIENT.MEMBERSHIPS.PURCHASE_ERROR');
          this.toaster.error(msg);
        },
      });
  }

  private loadPlans(): void {
    const gymId = this.profileService.profile()?.gymId;
    if (!gymId) {
      this.loading = false;
      return;
    }

    const req = new ListMembershipPlansRequest();
    req.gymId = gymId;
    req.paging.pageSize = 50;

    this.plansApi
      .list(req)
      .pipe(catchError(() => of({ items: [] } as any)))
      .subscribe({
        next: (res) => {
          this.plans = res.items ?? [];
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }
}
