import { Component, OnInit, inject, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ListUsersQueryDto, ListUsersRequest } from '../../../../api-services/users/users-api.models';
import { ADMIN_ROLE_ID, MEMBER_ROLE_ID, TRAINER_ROLE_ID } from '../../../auth/constants/auth.constants';
import { catchError, of } from 'rxjs';

export interface AddTrainerDialogData {
  gymId: number;
  existingTrainerUserPublicIds: string[];
}

export interface AddTrainerDialogResult {
  user: ListUsersQueryDto;
  bio: string;
  experienceYears: number;
}

@Component({
  selector: 'app-add-trainer-dialog',
  standalone: false,
  templateUrl: './add-trainer-dialog.component.html',
  styleUrl: './add-trainer-dialog.component.scss',
})
export class AddTrainerDialogComponent implements OnInit {
  private usersApi = inject(UsersApiService);
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<AddTrainerDialogComponent>);

  loading = true;
  search = '';
  users: ListUsersQueryDto[] = [];
  selectedUserPublicId: string | null = null;

  detailsForm = this.fb.group({
    bio: ['', [Validators.required, Validators.minLength(2)]],
    experienceYears: [0, [Validators.required, Validators.min(0)]],
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: AddTrainerDialogData) {}

  ngOnInit(): void {
    const req = new ListUsersRequest();
    req.gymId = this.data.gymId;
    req.paging.pageSize = 500;

    const existing = new Set(this.data.existingTrainerUserPublicIds);

    this.usersApi
      .list(req)
      .pipe(catchError(() => of({ items: [] })))
      .subscribe({
        next: (res) => {
          this.users = (res.items ?? []).filter(
            (u) =>
              !existing.has(u.publicId) &&
              u.roleId !== ADMIN_ROLE_ID &&
              (u.roleId === MEMBER_ROLE_ID || u.roleId === TRAINER_ROLE_ID),
          );
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
  }

  get filteredUsers(): ListUsersQueryDto[] {
    const q = this.search.trim().toLowerCase();
    if (!q) return this.users;
    return this.users.filter((u) => {
      const hay = `${u.firstName} ${u.lastName} ${u.email}`.toLowerCase();
      return hay.includes(q);
    });
  }

  get selectedUser(): ListUsersQueryDto | null {
    if (!this.selectedUserPublicId) return null;
    return this.users.find((u) => u.publicId === this.selectedUserPublicId) ?? null;
  }

  selectUser(userPublicId: string): void {
    this.selectedUserPublicId = userPublicId;
  }

  canApply(): boolean {
    return this.selectedUser != null && this.detailsForm.valid;
  }

  cancel(): void {
    this.dialogRef.close();
  }

  apply(): void {
    const user = this.selectedUser;
    if (!user || this.detailsForm.invalid) {
      this.detailsForm.markAllAsTouched();
      return;
    }

    const { bio, experienceYears } = this.detailsForm.getRawValue();
    const result: AddTrainerDialogResult = {
      user,
      bio: (bio ?? '').trim(),
      experienceYears: Number(experienceYears) || 0,
    };
    this.dialogRef.close(result);
  }
}
