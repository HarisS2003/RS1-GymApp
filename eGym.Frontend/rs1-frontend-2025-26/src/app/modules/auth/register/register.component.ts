import { Component, inject, OnInit } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ValidationErrors,
  Validators,
} from '@angular/forms';
import { Router } from '@angular/router';
import { switchMap } from 'rxjs';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { LoginCommand } from '../../../api-services/auth/auth-api.model';
import { UsersApiService } from '../../../api-services/users/users-api.service';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { GymSelectionService } from '../../public/services/gym-selection.service';
import { ADMIN_ROLE_ID, MEMBER_ROLE_ID } from '../constants/auth.constants';
import { UserProfileService } from '../../../core/services/user-profile.service';
import { TranslateService } from '@ngx-translate/core';

function passwordsMatch(control: AbstractControl): ValidationErrors | null {
  const password = control.get('password')?.value;
  const confirm = control.get('confirmPassword')?.value;
  return password === confirm ? null : { passwordMismatch: true };
}

@Component({
  selector: 'app-register',
  standalone: false,
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss',
})
export class RegisterComponent extends BaseComponent implements OnInit {
  private fb = inject(FormBuilder);
  private usersApi = inject(UsersApiService);
  private auth = inject(AuthFacadeService);
  private router = inject(Router);
  private currentUser = inject(CurrentUserService);
  private gymSelection = inject(GymSelectionService);
  private profileService = inject(UserProfileService);
  private translate = inject(TranslateService);

  hidePassword = true;
  hideConfirmPassword = true;

  form = this.fb.group(
    {
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]],
    },
    { validators: passwordsMatch },
  );

  ngOnInit(): void {
    if (!this.gymSelection.getSelectedGym()) {
      this.router.navigate(['/']);
    }
  }

  passwordStrength(): { key: string; className: string; percent: number } {
    const password = this.form.get('password')?.value ?? '';
    if (!password) return { key: 'AUTH.PASSWORD_STRENGTH.EMPTY', className: 'empty', percent: 0 };

    let score = 0;
    if (password.length >= 6) score++;
    if (password.length >= 10) score++;
    if (/[a-z]/.test(password) && /[A-Z]/.test(password)) score++;
    if (/\d/.test(password)) score++;
    if (/[^A-Za-z0-9]/.test(password)) score++;

    if (score <= 2) return { key: 'AUTH.PASSWORD_STRENGTH.WEAK', className: 'weak', percent: 25 };
    if (score === 3) return { key: 'AUTH.PASSWORD_STRENGTH.FAIR', className: 'fair', percent: 50 };
    if (score === 4) return { key: 'AUTH.PASSWORD_STRENGTH.GOOD', className: 'good', percent: 75 };
    return { key: 'AUTH.PASSWORD_STRENGTH.STRONG', className: 'strong', percent: 100 };
  }

  hasPasswordValue(): boolean {
    return !!this.form.get('password')?.value;
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) return;

    const gym = this.gymSelection.getSelectedGym();
    if (!gym) {
      this.router.navigate(['/']);
      return;
    }

    this.startLoading();

    const { firstName, lastName, email, password } = this.form.getRawValue();

    this.usersApi
      .create({
        firstName: firstName ?? '',
        lastName: lastName ?? '',
        email: email ?? '',
        password: password ?? '',
        roleId: MEMBER_ROLE_ID,
        gymId: gym.id,
      })
      .pipe(
        switchMap(() => {
          const loginPayload: LoginCommand = {
            email: email ?? '',
            password: password ?? '',
            fingerprint: null,
          };
          return this.auth.login(loginPayload);
        }),
        switchMap(() => this.profileService.loadProfile()),
      )
      .subscribe({
        next: (profile) => {
          if (profile) this.auth.applyRoleFromProfile(profile.roleId);
          this.stopLoading();
          const route =
            profile?.roleId === ADMIN_ROLE_ID
              ? '/admin/dashboard'
              : this.currentUser.getDefaultRoute();
          this.router.navigate([route]);
        },
        error: (err) => {
          const message =
            err?.error?.message ??
            err?.error?.title ??
            this.translate.instant('AUTH.REGISTER_ERROR');
          this.stopLoading(message);
        },
      });
  }
}
