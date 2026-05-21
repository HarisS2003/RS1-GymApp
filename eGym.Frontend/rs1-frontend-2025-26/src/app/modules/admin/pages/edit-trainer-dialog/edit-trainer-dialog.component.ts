import { Component, OnInit, inject, Inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { AdminTrainerRow } from '../admin-dashboard/admin-dashboard.component';

export interface EditTrainerDialogData {
  row: AdminTrainerRow;
}

@Component({
  selector: 'app-edit-trainer-dialog',
  standalone: false,
  templateUrl: './edit-trainer-dialog.component.html',
  styleUrl: './edit-trainer-dialog.component.scss',
})
export class EditTrainerDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private trainersApi = inject(TrainersApiService);
  private dialogRef = inject(MatDialogRef<EditTrainerDialogComponent>);
  private toaster = inject(ToasterService);
  private translate = inject(TranslateService);

  saving = false;
  form = this.fb.group({
    bio: ['', [Validators.required, Validators.minLength(2)]],
    experienceYears: [0, [Validators.required, Validators.min(0)]],
  });

  constructor(@Inject(MAT_DIALOG_DATA) public data: EditTrainerDialogData) {}

  ngOnInit(): void {
    this.form.patchValue({
      bio: this.data.row.trainer.bio ?? '',
      experienceYears: this.data.row.trainer.experienceYears ?? 0,
    });
  }

  cancel(): void {
    this.dialogRef.close(false);
  }

  apply(): void {
    if (this.form.invalid || this.saving) return;
    const { bio, experienceYears } = this.form.getRawValue();
    const t = this.data.row.trainer;
    this.saving = true;

    this.trainersApi
      .update(t.id, {
        userId: t.userId,
        gymId: t.gymId,
        bio: bio ?? '',
        experienceYears: Number(experienceYears) || 0,
      })
      .subscribe({
        next: () => this.dialogRef.close(true),
        error: (err) => {
          this.saving = false;
          const msg =
            err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('ADMIN_DASH.EDIT_TRAINER_ERROR');
          this.toaster.error(msg);
        },
      });
  }
}
