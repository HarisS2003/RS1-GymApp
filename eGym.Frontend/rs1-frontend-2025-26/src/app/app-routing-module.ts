import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { withPageAnimation } from './core/animations/route-animations';
import {myAuthData, myAuthGuard} from './core/guards/my-auth-guard';

const routes: Routes = [
  {
    path: 'admin',
    canActivate: [myAuthGuard],
    data: { ...myAuthData({ requireAuth: true, requireAdmin: true }), ...withPageAnimation('AdminArea') },
    loadChildren: () =>
      import('./modules/admin/admin-module').then(m => m.AdminModule)
  },
  {
    path: 'auth',
    data: withPageAnimation('AuthArea'),
    loadChildren: () =>
      import('./modules/auth/auth-module').then(m => m.AuthModule)
  },
  {
    path: 'client',
    canActivate: [myAuthGuard],
    data: { ...myAuthData({ requireAuth: true }), ...withPageAnimation('ClientArea') },
    loadChildren: () =>
      import('./modules/client/client-module').then(m => m.ClientModule)
  },
  {
    path: '',
    data: withPageAnimation('PublicArea'),
    loadChildren: () =>
      import('./modules/public/public-module').then(m => m.PublicModule)
  },
  // fallback 404
  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule {}
