import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientLayoutComponent } from './layout/client-layout.component';
import { ClientHomeComponent } from './pages/home/client-home.component';
import { ClientTrainersComponent } from './pages/trainers/client-trainers.component';
import { ClientShopComponent } from './pages/shop/client-shop.component';
import { ClientMembershipsComponent } from './pages/memberships/client-memberships.component';
import { ClientProfileComponent } from './pages/profile/client-profile.component';
import { ClientScheduleComponent } from './pages/schedule/client-schedule.component';
import { ClientTrainerBookingComponent } from './pages/trainer-booking/client-trainer-booking.component';

const routes: Routes = [
  {
    path: '',
    component: ClientLayoutComponent,
    children: [
      { path: '', component: ClientHomeComponent },
      { path: 'trainers', component: ClientTrainersComponent },
      { path: 'trainers/:trainerId/book', component: ClientTrainerBookingComponent },
      { path: 'shop', component: ClientShopComponent },
      { path: 'memberships', component: ClientMembershipsComponent },
      { path: 'profile', component: ClientProfileComponent },
      { path: 'schedule', component: ClientScheduleComponent },
      { path: '**', redirectTo: '' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
