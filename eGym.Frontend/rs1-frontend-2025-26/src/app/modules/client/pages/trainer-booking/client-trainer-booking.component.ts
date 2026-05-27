import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { TranslateService } from '@ngx-translate/core';
import { TrainingRequestsApiService } from '../../../../api-services/training-requests/training-requests-api.service';
import { TrainerAvailableSlotDto } from '../../../../api-services/training-requests/training-requests-api.models';
import { TrainersApiService } from '../../../../api-services/trainers/trainers-api.service';
import { UsersApiService } from '../../../../api-services/users/users-api.service';
import { ListTrainersRequest } from '../../../../api-services/trainers/trainers-api.models';
import { ListTrainersQueryDto } from '../../../../api-services/trainers/trainers-api.models';
import { GetUserByIdQueryDto } from '../../../../api-services/users/users-api.models';
import { UserProfileService } from '../../../../core/services/user-profile.service';
import { ToasterService } from '../../../../core/services/toaster.service';
import { UserMembershipsApiService } from '../../../../api-services/user-memberships/user-memberships-api.service';

@Component({
  selector: 'app-client-trainer-booking',
  standalone: false,
  templateUrl: './client-trainer-booking.component.html',
  styleUrl: './client-trainer-booking.component.scss',
})
export class ClientTrainerBookingComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private trainersApi = inject(TrainersApiService);
  private usersApi = inject(UsersApiService);
  private requestsApi = inject(TrainingRequestsApiService);
  private translate = inject(TranslateService);
  private toaster = inject(ToasterService);
  private membershipsApi = inject(UserMembershipsApiService);
  profileService = inject(UserProfileService);

  loading = true;
  membershipChecked = false;
  hasActiveMembership = false;
  slotsLoading = false;
  booking = false;
  trainerId = 0;
  trainer: ListTrainersQueryDto | null = null;
  trainerUser: GetUserByIdQueryDto | null = null;
  selectedDate: Date | null = null;
  selectedSlot: string | null = null;
  slots: TrainerAvailableSlotDto[] = [];
  minDate = new Date();

  ngOnInit(): void {
    this.trainerId = Number(this.route.snapshot.paramMap.get('trainerId'));
    if (!this.trainerId) {
      this.router.navigate(['/client/trainers']);
      return;
    }
    this.membershipsApi.getMyActive().subscribe({
      next: (active) => {
        this.hasActiveMembership = !!active;
        this.membershipChecked = true;
        this.loadTrainer();
      },
      error: () => {
        this.hasActiveMembership = false;
        this.membershipChecked = true;
        this.loadTrainer();
      },
    });
  }

  trainerName(): string {
    if (!this.trainerUser) return `#${this.trainerId}`;
    return `${this.trainerUser.firstName} ${this.trainerUser.lastName}`.trim();
  }

  onDateChange(date: Date | null): void {
    this.selectedDate = date;
    this.selectedSlot = null;
    this.slots = [];
    if (!date) return;
    this.loadSlots(date);
  }

  selectSlot(slot: TrainerAvailableSlotDto): void {
    if (this.booking) return;
    this.selectedSlot = slot.startTime;
  }

  submitBooking(): void {
    if (!this.selectedDate || !this.selectedSlot || this.booking) return;
    if (!this.hasActiveMembership) {
      this.toaster.error(this.translate.instant('CLIENT.TRAINER_BOOKING.MEMBERSHIP_REQUIRED'));
      return;
    }

    this.booking = true;
    const dateIso = this.toDateOnly(this.selectedDate);
    const startTime = this.toTimeSpan(this.selectedSlot);

    this.requestsApi
      .create({
        trainerId: this.trainerId,
        date: dateIso,
        startTime,
      })
      .subscribe({
        next: () => {
          this.booking = false;
          this.toaster.success(this.translate.instant('CLIENT.TRAINER_BOOKING.SUCCESS'));
          this.router.navigate(['/client/profile']);
        },
        error: (err) => {
          this.booking = false;
          this.toaster.error(
            err?.error?.message ??
              err?.error?.title ??
              this.translate.instant('CLIENT.TRAINER_BOOKING.ERROR'),
          );
        },
      });
  }

  back(): void {
    this.router.navigate(['/client/trainers']);
  }

  selectedDateLabel(): string {
    if (!this.selectedDate) return '';
    return this.selectedDate.toLocaleDateString('bs-BA', {
      day: 'numeric',
      month: 'long',
      year: 'numeric',
    });
  }

  private loadTrainer(): void {
    const req = new ListTrainersRequest();
    req.paging.pageSize = 200;

    this.trainersApi
      .list(req)
      .pipe(catchError(() => of({ items: [] } as any)))
      .subscribe({
        next: (res) => {
          const found = (res.items ?? []).find((t: ListTrainersQueryDto) => t.id === this.trainerId);
          if (!found) {
            this.loading = false;
            return;
          }
          this.trainer = found;
          this.usersApi.getById(found.userId).subscribe({
            next: (user) => {
              this.trainerUser = user;
              this.loading = false;
            },
            error: () => (this.loading = false),
          });
        },
        error: () => (this.loading = false),
      });
  }

  private loadSlots(date: Date): void {
    this.slotsLoading = true;
    this.requestsApi
      .getAvailableSlots(this.trainerId, this.toDateOnly(date))
      .pipe(catchError(() => of([])))
      .subscribe({
        next: (slots) => {
          this.slots = slots;
          this.slotsLoading = false;
        },
        error: () => {
          this.slots = [];
          this.slotsLoading = false;
        },
      });
  }

  private toDateOnly(d: Date): string {
    const y = d.getFullYear();
    const m = String(d.getMonth() + 1).padStart(2, '0');
    const day = String(d.getDate()).padStart(2, '0');
    return `${y}-${m}-${day}`;
  }

  private toTimeSpan(hhmm: string): string {
    const parts = hhmm.split(':');
    const h = parts[0] ?? '00';
    const m = parts[1] ?? '00';
    return `${h.padStart(2, '0')}:${m.padStart(2, '0')}:00`;
  }
}
