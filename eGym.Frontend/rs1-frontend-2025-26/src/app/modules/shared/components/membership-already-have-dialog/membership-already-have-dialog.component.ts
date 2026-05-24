import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { GetMyActiveUserMembershipQueryDto } from '../../../../api-services/user-memberships/user-memberships-api.models';

export interface MembershipAlreadyHaveDialogData {
  active: GetMyActiveUserMembershipQueryDto;
  selectedDurationDays: number;
  isUpgradeBlocked: boolean;
}

@Component({
  selector: 'app-membership-already-have-dialog',
  standalone: false,
  templateUrl: './membership-already-have-dialog.component.html',
  styleUrl: './membership-already-have-dialog.component.scss',
})
export class MembershipAlreadyHaveDialogComponent {
  private dialogRef = inject(MatDialogRef<MembershipAlreadyHaveDialogComponent>);
  data = inject<MembershipAlreadyHaveDialogData>(MAT_DIALOG_DATA);

  close(): void {
    this.dialogRef.close();
  }
}
