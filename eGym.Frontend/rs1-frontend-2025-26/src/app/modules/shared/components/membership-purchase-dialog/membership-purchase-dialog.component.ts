import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ListMembershipPlansQueryDto } from '../../../../api-services/membership-plans/membership-plans-api.models';

export interface MembershipPurchaseDialogData {
  plan: ListMembershipPlansQueryDto;
  finalPrice: number;
  durationLabel: string;
  isUpgrade?: boolean;
  currentPlanName?: string;
  currentDurationDays?: number;
}

export interface MembershipPurchaseDialogResult {
  confirmed: boolean;
}

@Component({
  selector: 'app-membership-purchase-dialog',
  standalone: false,
  templateUrl: './membership-purchase-dialog.component.html',
  styleUrl: './membership-purchase-dialog.component.scss',
})
export class MembershipPurchaseDialogComponent {
  private dialogRef = inject(MatDialogRef<MembershipPurchaseDialogComponent, MembershipPurchaseDialogResult>);
  data = inject<MembershipPurchaseDialogData>(MAT_DIALOG_DATA);

  get plan(): ListMembershipPlansQueryDto {
    return this.data.plan;
  }

  get hasDiscount(): boolean {
    return this.plan.discountPercentage > 0;
  }

  cancel(): void {
    this.dialogRef.close({ confirmed: false });
  }

  confirm(): void {
    this.dialogRef.close({ confirmed: true });
  }
}
