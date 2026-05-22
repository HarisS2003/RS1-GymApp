import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MembershipPlanFormService } from '../services/membership-plan-form.service';
import { BaseFormComponent } from '../../../../../core/components/base-classes/base-form-component';
import {
  GetMembershipPlanByIdQueryDto,
  UpdateMembershipPlanCommand,
} from '../../../../../api-services/membership-plans/membership-plans-api.models';
import { MembershipPlansApiService } from '../../../../../api-services/membership-plans/membership-plans-api.service';
import { ToasterService } from '../../../../../core/services/toaster.service';
import { UserProfileService } from '../../../../../core/services/user-profile.service';

@Component({
  selector: 'app-membership-plans-edit',
  standalone: false,
  templateUrl: './membership-plans-edit.component.html',
  styleUrl: './membership-plans-edit.component.scss',
  providers: [MembershipPlanFormService],
})
export class MembershipPlansEditComponent
  extends BaseFormComponent<GetMembershipPlanByIdQueryDto>
  implements OnInit
{
  private api = inject(MembershipPlansApiService);
  private formService = inject(MembershipPlanFormService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private toaster = inject(ToasterService);
  private profileService = inject(UserProfileService);

  planId!: number;

  ngOnInit(): void {
    this.planId = +this.route.snapshot.params['id'];
    this.initForm(true);
  }

  protected loadData(): void {
    this.startLoading();
    this.api.getById(this.planId).subscribe({
      next: (plan) => {
        this.model = plan;
        this.form = this.formService.createForm(plan);
        this.stopLoading();
      },
      error: () => {
        this.stopLoading('Failed to load membership plan');
        this.toaster.error('Membership plan not found');
        this.router.navigate(['/admin/membership-plans']);
      },
    });
  }

  protected save(): void {
    if (this.form.invalid || this.isLoading) return;

    const gymId = this.model?.gymId ?? this.profileService.profile()?.gymId;
    if (!gymId) {
      this.toaster.error('No gym on profile');
      return;
    }

    this.startLoading();
    const v = this.form.getRawValue();
    const command: UpdateMembershipPlanCommand = {
      name: v.name,
      durationDays: v.durationDays,
      price: v.price,
      discountPercentage: v.discountPercentage ?? 0,
      gymId,
    };

    this.api.update(this.planId, command).subscribe({
      next: () => {
        this.stopLoading();
        this.toaster.success('Membership plan updated');
        this.router.navigate(['/admin/membership-plans']);
      },
      error: (err) => {
        this.stopLoading('Failed to update membership plan');
        const msg =
          err?.error?.detail ?? err?.error?.message ?? 'Could not update membership plan';
        this.toaster.error(msg);
      },
    });
  }

  onCancel(): void {
    this.router.navigate(['/admin/membership-plans']);
  }

  getErrorMessage(controlName: string): string {
    return this.formService.getErrorMessage(this.form, controlName);
  }
}
