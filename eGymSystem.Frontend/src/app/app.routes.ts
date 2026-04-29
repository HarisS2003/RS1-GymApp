import { Routes } from '@angular/router';
import { CrudPageComponent } from './features/crud/crud-page.component';

export const routes: Routes = [
  { path: '', redirectTo: 'users', pathMatch: 'full' },
  { path: 'users', component: CrudPageComponent, data: { resource: 'Users' } },
  { path: 'trainers', component: CrudPageComponent, data: { resource: 'Trainers' } },
  { path: 'trainings', component: CrudPageComponent, data: { resource: 'Trainings' } },
  { path: 'training-requests', component: CrudPageComponent, data: { resource: 'Training Requests' } },
  { path: 'products', component: CrudPageComponent, data: { resource: 'Products' } },
  { path: 'orders', component: CrudPageComponent, data: { resource: 'Orders' } },
  { path: 'memberships', component: CrudPageComponent, data: { resource: 'Memberships' } },
];
