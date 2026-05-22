import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { MembershipPlanFormService } from '../services/membership-plan-form.service';
import {
  CreateMembershipPlanCommand,
  GetMembershipPlanByIdQueryDto,
} from '../../../../../api-services/membership-plans/membership-plans-api.models';
import { MembershipPlansApiService } from '../../../../../api-services/membership-plans/membership-plans-api.service';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { UserProfileService } from '../../../../../core/services/user-profile.service';

@Component({
  selector: 'app-membership-plans-add',
  standalone: false,
  templateUrl: './membership-plans-add.component.html',
  styleUrl: './membership-plans-add.component.scss',
  providers: [MembershipPlanFormService],
})
export class MembershipPlansAddComponent
  extends BaseFormComponent<GetMembershipPlanByIdQueryDto>
  implements OnInit
{
  private api = inject(MembershipPlansApiService);
  private formService = inject(MembershipPlanFormService);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private profileService = inject(UserProfileService);

  ngOnInit(): void {
    this.initForm(false);
  }

  protected loadData(): void {}

  protected save(): void {
    if (this.form.invalid || this.isLoading) return;

    const gymId = this.profileService.profile()?.gymId;
    if (!gymId) {
      this.toaster.error('No gym on admin profile');
      return;
    }

    this.startLoading();
    const v = this.form.getRawValue();
    const command: CreateMembershipPlanCommand = {
      name: v.name,
      durationDays: v.durationDays,
      price: v.price,
      discountPercentage: v.discountPercentage ?? 0,
      gymId,
    };

    this.api.create(command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Membership plan created');
        this.router.navigate(['/admin/membership-plans']);
      },
      error: (err) => {
        this.stopLoading('Failed to create membership plan');
        const msg =
          err?.error?.detail ?? err?.error?.message ?? 'Could not create membership plan';
        this.toaster.error(msg);
      },
    });
  }

  protected override initForm(isEdit: boolean): void {
    super.initForm(isEdit);
    this.form = this.formService.createForm();
  }

  onCancel(): void {
    this.router.navigate(['/admin/membership-plans']);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
