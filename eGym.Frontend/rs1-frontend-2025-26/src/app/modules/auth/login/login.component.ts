import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BaseComponent } from '../../../core/components/base-classes/base-component';
import { AuthFacadeService } from '../../../core/services/auth/auth-facade.service';
import { LoginCommand } from '../../../api-services/auth/auth-api.model';
import { CurrentUserService } from '../../../core/services/auth/current-user.service';
import { GymSelectionService } from '../../public/services/gym-selection.service';
import { UserProfileService } from '../../../core/services/user-profile.service';
import { ADMIN_ROLE_ID, DEMO_LOGIN } from '../constants/auth.constants';
import { switchMap } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent extends BaseComponent implements OnInit {
  private fb = inject(FormBuilder);
  private auth = inject(AuthFacadeService);
  private router = inject(Router);
  private currentUser = inject(CurrentUserService);
  private gymSelection = inject(GymSelectionService);
  private profileService = inject(UserProfileService);
  private translate = inject(TranslateService);

  readonly demo = DEMO_LOGIN;
  hidePassword = true;

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [false],
  });

  ngOnInit(): void {
    if (!this.gymSelection.getSelectedGym()) {
      this.router.navigate(['/']);
      return;
    }

    const remembered = localStorage.getItem('rememberedEmail');
    if (remembered) {
      this.form.patchValue({ email: remembered, rememberMe: true });
    }
  }

  onSubmit(): void {
    if (this.form.invalid || this.isLoading) return;

    this.startLoading();

    const email = this.form.value.email ?? '';
    const password = this.form.value.password ?? '';

    if (this.form.value.rememberMe) {
      localStorage.setItem('rememberedEmail', email);
    } else {
      localStorage.removeItem('rememberedEmail');
    }

    const payload: LoginCommand = {
      email,
      password,
      fingerprint: null,
    };

    this.auth
      .login(payload)
      .pipe(switchMap(() => this.profileService.loadProfile()))
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
      error: () => {
        this.stopLoading(this.translate.instant('AUTH.LOGIN_ERROR'));
      },
    });
  }
}
