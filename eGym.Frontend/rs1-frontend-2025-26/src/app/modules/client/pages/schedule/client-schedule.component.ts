import { Component } from '@angular/core';

/** Trainer schedule — same data block as trainer home list */
@Component({
  selector: 'app-client-schedule',
  standalone: false,
  template: `<app-trainer-home></app-trainer-home>`,
})
export class ClientScheduleComponent {}
