import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { withPageAnimation } from '../../core/animations/route-animations';
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
      { path: '', component: ClientHomeComponent, data: withPageAnimation('ClientHome') },
      { path: 'trainers', component: ClientTrainersComponent, data: withPageAnimation('ClientTrainers') },
      {
        path: 'trainers/:trainerId/book',
        component: ClientTrainerBookingComponent,
        data: withPageAnimation('ClientTrainerBooking'),
      },
      { path: 'shop', component: ClientShopComponent, data: withPageAnimation('ClientShop') },
      { path: 'memberships', component: ClientMembershipsComponent, data: withPageAnimation('ClientMemberships') },
      { path: 'profile', component: ClientProfileComponent, data: withPageAnimation('ClientProfile') },
      { path: 'schedule', component: ClientScheduleComponent, data: withPageAnimation('ClientSchedule') },
      { path: '**', redirectTo: '' },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ClientRoutingModule {}
