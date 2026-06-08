import { Component, OnInit, inject, Inject } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { UserMembershipsApiService } from '../../../../api-services/user-memberships/user-memberships-api.service';
import {
  GetMembershipHistoryQueryDto,
  MembershipEventTimelineItemDto,
  MembershipHistoryStateDto,
} from '../../../../api-services/user-memberships/user-memberships-api.models';
import { ToasterService } from '../../../../core/services/toaster.service';

export interface MembershipHistoryDialogData {
  membershipPublicId: string;
  userName: string;
  planName: string;
}

type TimelineTone = 'created' | 'frozen' | 'activated' | 'adjusted' | 'default';

@Component({
  selector: 'app-membership-history-dialog',
  standalone: false,
  templateUrl: './membership-history-dialog.component.html',
  styleUrl: './membership-history-dialog.component.scss',
})
export class MembershipHistoryDialogComponent implements OnInit {
  private membershipsApi = inject(UserMembershipsApiService);
  private dialogRef = inject(MatDialogRef<MembershipHistoryDialogComponent>);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);
  private fb = inject(FormBuilder);

  loading = true;
  history: GetMembershipHistoryQueryDto | null = null;

  form = this.fb.group({
    asOfDate: [this.toLocalInputValue(new Date())],
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: MembershipHistoryDialogData) {}

  ngOnInit(): void {
    this.loadHistory();
  }

  close(): void {
    this.dialogRef.close();
  }

  travel(): void {
    this.loadHistory();
  }

  eventLabel(event: MembershipEventTimelineItemDto): string {
    const key = `ADMIN_USERS.EVENT_${event.eventType.toUpperCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : event.eventType;
  }

  statusLabel(status: string): string {
    const key = `ADMIN_USERS.STATUS_${status.toUpperCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : status;
  }

  statusClass(status: string): string {
    switch (status) {
      case 'Active':
        return 'status-pill';
      case 'Frozen':
        return 'status-pill status-pill--frozen';
      case 'Expired':
        return 'status-pill status-pill--inactive';
      default:
        return 'status-pill status-pill--none';
    }
  }

  eventTone(eventType: string): TimelineTone {
    switch (eventType) {
      case 'MembershipCreated':
        return 'created';
      case 'MembershipFrozen':
        return 'frozen';
      case 'MembershipActivated':
        return 'activated';
      case 'MembershipEndDateAdjusted':
        return 'adjusted';
      default:
        return 'default';
    }
  }

  eventMeta(event: MembershipEventTimelineItemDto): string[] {
    try {
      const data = JSON.parse(event.eventData) as Record<string, unknown>;
      const lines: string[] = [];

      if (typeof data['reason'] === 'string' && data['reason']) {
        lines.push(
          this.translate.instant('ADMIN_USERS.META_REASON', { reason: data['reason'] }),
        );
      }

      if (data['amountPaid'] != null) {
        lines.push(
          this.translate.instant('ADMIN_USERS.META_PRICE', { amount: data['amountPaid'] }),
        );
      }

      if (typeof data['planName'] === 'string' && data['planName']) {
        lines.push(
          this.translate.instant('ADMIN_USERS.META_PLAN', { plan: data['planName'] }),
        );
      }

      if (data['endDate']) {
        const end = this.formatDate(String(data['endDate']));
        if (end) {
          lines.push(this.translate.instant('ADMIN_USERS.META_END_DATE', { date: end }));
        }
      }

      return lines;
    } catch {
      return [];
    }
  }

  showPeriod(state: MembershipHistoryStateDto): boolean {
    return !!state.hasMembership && !!state.periodDisplay;
  }

  private loadHistory(): void {
    const asOfDate = this.toInclusiveAsOfIso(this.form.value.asOfDate);
    this.loading = true;

    this.membershipsApi.getHistory(this.data.membershipPublicId, asOfDate).subscribe({
      next: (dto) => {
        this.history = dto;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toaster.error(this.translate.instant('ADMIN_USERS.HISTORY_ERROR'));
      },
    });
  }

  private formatDate(value: string): string | null {
    const date = new Date(value);
    if (Number.isNaN(date.getTime()) || date.getFullYear() < 1900) {
      return null;
    }
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${pad(date.getDate())}.${pad(date.getMonth() + 1)}.${date.getFullYear()}`;
  }

  /** End of selected local calendar day → avoids cutting same-day events by timezone. */
  private toInclusiveAsOfIso(raw: string | null | undefined): string | undefined {
    if (!raw) return undefined;

    const [datePart] = raw.split('T');
    const [year, month, day] = datePart.split('-').map(Number);
    if (!year || !month || !day) return undefined;

    return new Date(year, month - 1, day, 23, 59, 59, 999).toISOString();
  }

  private toLocalInputValue(date: Date): string {
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
  }
}
