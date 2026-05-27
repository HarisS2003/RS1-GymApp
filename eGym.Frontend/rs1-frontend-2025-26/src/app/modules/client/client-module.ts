import { NgModule } from '@angular/core';
import { ClientRoutingModule } from './client-routing-module';
import { SharedModule } from '../shared/shared-module';
import { ClientLayoutComponent } from './layout/client-layout.component';
import { ClientHomeComponent } from './pages/home/client-home.component';
import { TrainerHomeComponent } from './pages/trainer-home/trainer-home.component';
import { ClientTrainersComponent } from './pages/trainers/client-trainers.component';
import { ClientShopComponent } from './pages/shop/client-shop.component';
import { ClientMembershipsComponent } from './pages/memberships/client-memberships.component';
import { ClientProfileComponent } from './pages/profile/client-profile.component';
import { ClientScheduleComponent } from './pages/schedule/client-schedule.component';
import { ClientTrainerBookingComponent } from './pages/trainer-booking/client-trainer-booking.component';
import { GroupTrainingDialogComponent } from './pages/trainer-home/group-training-dialog.component';

@NgModule({
  declarations: [
    ClientLayoutComponent,
    ClientHomeComponent,
    TrainerHomeComponent,
    ClientTrainersComponent,
    ClientShopComponent,
    ClientMembershipsComponent,
    ClientProfileComponent,
    ClientScheduleComponent,
    ClientTrainerBookingComponent,
    GroupTrainingDialogComponent,
  ],
  imports: [SharedModule, ClientRoutingModule],
})
export class ClientModule {}
