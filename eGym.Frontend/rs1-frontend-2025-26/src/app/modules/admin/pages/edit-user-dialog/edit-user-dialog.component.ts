import { Component, OnInit, inject, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { bosnianPhoneValidator } from '../../../../core/validators/bosnian-phone.validator';

export interface EditUserDialogData {
  userId: number;
}

@Component({
  selector: 'app-edit-user-dialog',
  standalone: false,
  templateUrl: './edit-user-dialog.component.html',
  styleUrl: './edit-user-dialog.component.scss',
})
export class EditUserDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private usersApi = inject(UsersApiService);
  private dialogRef = inject(MatDialogRef<EditUserDialogComponent>);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  saving = false;
  loading = true;

  form = this.fb.group({
    firstName: ['', [Validators.required]],
    lastName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: ['', [Validators.required, bosnianPhoneValidator]],
    roleId: [0, [Validators.required]],
    gymId: [0, [Validators.required]],
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: EditUserDialogData) {}

  ngOnInit(): void {
    this.usersApi.getById(this.data.userId).subscribe({
      next: (user) => {
        this.form.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email,
          phoneNumber: user.phoneNumber,
          roleId: user.roleId,
          gymId: user.gymId,
        });
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.toaster.error(this.translate.instant('ADMIN_USERS.EDIT_LOAD_ERROR'));
        this.dialogRef.close(false);
      },
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  apply(): void {
    if (this.form.invalid || this.saving) return;
    this.saving = true;
    const value = this.form.getRawValue();

    this.usersApi
      .update(this.data.userId, {
        firstName: value.firstName ?? '',
        lastName: value.lastName ?? '',
        email: value.email ?? '',
        phoneNumber: value.phoneNumber ?? '',
        roleId: Number(value.roleId),
        gymId: Number(value.gymId),
      })
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.saving = false;
          const msg =
            err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('ADMIN_USERS.EDIT_ERROR');
          this.toaster.error(msg);
        },
      });
  }
}
