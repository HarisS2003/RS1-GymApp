import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ListGymsQueryDto, ListGymsRequest } from '../../../api-services/gyms/gyms-api.models';
import { GymsApiService } from '../../../api-services/gyms/gyms-api.service';
import { GymSelectionService } from '../services/gym-selection.service';

@Component({
  selector: 'app-public-layout',
  standalone: false,
  templateUrl: './public-layout.component.html',
  styleUrl: './public-layout.component.scss',
})
export class PublicLayoutComponent implements OnInit {
  private readonly gymsApi = inject(GymsApiService);
  private readonly gymSelection = inject(GymSelectionService);
  private readonly router = inject(Router);

  gyms: ListGymsQueryDto[] = [];
  loading = true;
  loadFailed = false;

  get gymCount(): number {
    return this.gyms.length;
  }

  get cityCount(): number {
    return new Set(this.gyms.map((g) => g.city?.trim()).filter(Boolean)).size;
  }

  ngOnInit(): void {
    this.loadGyms();
  }

  onGymSelected(gym: ListGymsQueryDto): void {
    this.gymSelection.selectGym(gym);
    this.router.navigate(['/auth/login']);
  }

  retryLoad(): void {
    this.loadGyms();
  }

  private loadGyms(): void {
    this.loading = true;
    this.loadFailed = false;

    const request = new ListGymsRequest();
    request.paging.page = 1;
    request.paging.pageSize = 500;

    this.gymsApi.list(request).subscribe({
      next: (result) => {
        this.gyms = result.items ?? [];
        this.loading = false;
      },
      error: () => {
        this.gyms = [];
        this.loadFailed = true;
        this.loading = false;
      },
    });
  }
}
