import { Injectable, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { GetMembershipPlanByIdQueryDto } from '../../../../../api-services/membership-plans/membership-plans-api.models';

@Injectable()
export class MembershipPlanFormService {
  private fb = inject(FormBuilder);

  createForm(plan?: GetMembershipPlanByIdQueryDto): FormGroup {
    return this.fb.group({
      name: [
        plan?.name ?? '',
        [Validators.required, Validators.minLength(2), Validators.maxLength(150)],
      ],
      durationDays: [
        plan?.durationDays ?? 30,
        [Validators.required, Validators.min(1), Validators.max(3650)],
      ],
      price: [
        plan?.price ?? 0,
        [Validators.required, Validators.min(0.01), Validators.max(1000000)],
      ],
      discountPercentage: [
        plan?.discountPercentage ?? 0,
        [Validators.required, Validators.min(0), Validators.max(100)],
      ],
    });
  }

  getErrorMessage(form: FormGroup, controlName: string): string {
    const control = form.get(controlName);
    if (!control || !control.errors || !control.touched) return '';

    const errors = control.errors;
    if (errors['required']) return 'This field is required';
    if (errors['minlength'])
      return `Minimum ${errors['minlength'].requiredLength} characters required`;
    if (errors['maxlength'])
      return `Maximum ${errors['maxlength'].requiredLength} characters allowed`;
    if (errors['min']) return `Minimum value is ${errors['min'].min}`;
    if (errors['max']) return `Maximum value is ${errors['max'].max}`;
    return 'Invalid value';
  }
}
