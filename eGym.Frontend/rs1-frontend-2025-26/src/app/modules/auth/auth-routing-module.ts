import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { withPageAnimation } from '../../core/animations/route-animations';
import {AuthLayoutComponent} from './auth-layout/auth-layout.component';
import {LoginComponent} from './login/login.component';
import {ForgotPasswordComponent} from './forgot-password/forgot-password.component';
import {RegisterComponent} from './register/register.component';
import {LogoutComponent} from './logout/logout.component';

const routes: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent, data: withPageAnimation('AuthLogin') },
      { path: 'register', component: RegisterComponent, data: withPageAnimation('AuthRegister') },
      { path: 'forgot-password', component: ForgotPasswordComponent, data: withPageAnimation('AuthForgotPassword') },
      { path: 'logout', component: LogoutComponent, data: withPageAnimation('AuthLogout') },
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
