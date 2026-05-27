import { Component, inject } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

export interface GroupTrainingDialogResult {
  description: string;
  date: string;
  startTime: string;
  capacity: number;
}

@Component({
  selector: 'app-group-training-dialog',
  standalone: false,
  templateUrl: './group-training-dialog.component.html',
  styleUrl: './group-training-dialog.component.scss',
})
export class GroupTrainingDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<GroupTrainingDialogComponent, GroupTrainingDialogResult>);

  minDate = new Date().toISOString().slice(0, 10);

  form = this.fb.group({
    description: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(1000)]],
    date: [this.minDate, Validators.required],
    startTime: ['18:00', Validators.required],
    capacity: [10, [Validators.required, Validators.min(2), Validators.max(100)]],
  });

  close(): void {
    this.dialogRef.close();
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const value = this.form.getRawValue();
    this.dialogRef.close({
      description: value.description ?? '',
      date: value.date ?? this.minDate,
      startTime: `${value.startTime}:00`,
      capacity: Number(value.capacity) || 2,
    });
  }
}
