import { Component, OnInit, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import {
  ListUsersWithMembershipQueryDto,
  ListUsersWithMembershipRequest,
} from '../../../../api-services/users/users-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { MEMBER_ROLE_ID } from '../../../auth/constants/auth.constants';
import {
  EditUserDialogComponent,
  EditUserDialogData,
} from '../edit-user-dialog/edit-user-dialog.component';
import {
  MembershipHistoryDialogComponent,
  MembershipHistoryDialogData,
} from '../membership-history-dialog/membership-history-dialog.component';

@Component({
  selector: 'app-admin-users',
  standalone: false,
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss',
})
export class AdminUsersComponent implements OnInit {
  private usersApi = inject(UsersApiService);
  private profileService = inject(UserProfileService);
  private toaster = inject(ToasterService);
  private dialog = inject(MatDialog);
  private translate = inject(TranslateService);

  loading = true;
  userRows: ListUsersWithMembershipQueryDto[] = [];
  readonly userTableColumns = ['id', 'name', 'email', 'membership', 'status', 'actions'];

  ngOnInit(): void {
    if (this.profileService.profile()) {
      this.load();
      return;
    }
    this.profileService.loadProfile().subscribe(() => this.load());
  }

  fullName(row: ListUsersWithMembershipQueryDto): string {
    return `${row.firstName} ${row.lastName}`.trim();
  }

  statusLabel(status: string): string {
    const key = `ADMIN_USERS.STATUS_${status.toUpperCase()}`;
    const translated = this.translate.instant(key);
    return translated !== key ? translated : status;
  }

  editUser(row: ListUsersWithMembershipQueryDto): void {
    this.dialog
      .open(EditUserDialogComponent, {
        width: '480px',
        data: { userId: row.id } satisfies EditUserDialogData,
      })
      .afterClosed()
      .subscribe((ok) => {
        if (ok) {
          this.toaster.success(this.translate.instant('ADMIN_USERS.EDIT_SUCCESS'));
          this.load();
        }
      });
  }

  deleteUser(row: ListUsersWithMembershipQueryDto): void {
    const name = this.fullName(row);
    const msg = this.translate.instant('ADMIN_USERS.DELETE_CONFIRM', { name });
    if (!confirm(msg)) return;

    this.usersApi.delete(row.id).subscribe({
      next: () => {
        this.toaster.success(this.translate.instant('ADMIN_USERS.DELETE_SUCCESS', { name }));
        this.load();
      },
      error: (err) => {
        const message =
          err?.error?.message ??
          err?.error?.title ??
          this.translate.instant('ADMIN_USERS.DELETE_ERROR');
        this.toaster.error(message);
      },
    });
  }

  openHistory(row: ListUsersWithMembershipQueryDto): void {
    if (!row.userMembershipId) {
      this.toaster.error(this.translate.instant('ADMIN_USERS.NO_MEMBERSHIP_HISTORY'));
      return;
    }

    const data: MembershipHistoryDialogData = {
      userMembershipId: row.userMembershipId,
      userName: this.fullName(row),
      planName: row.currentMembershipName ?? '—',
    };

    this.dialog.open(MembershipHistoryDialogComponent, {
      width: '640px',
      maxHeight: '90vh',
      data,
    });
  }

  private load(): void {
    const gymId = this.profileService.profile()?.gymId;
    const req = new ListUsersWithMembershipRequest();
    req.roleId = MEMBER_ROLE_ID;
    req.paging.pageSize = 500;
    if (gymId) req.gymId = gymId;

    this.loading = true;
    this.usersApi.listWithMemberships(req).subscribe({
      next: (res) => {
        this.userRows = res.items ?? [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toaster.error(this.translate.instant('ADMIN_USERS.LOAD_ERROR'));
      },
    });
  }
}
