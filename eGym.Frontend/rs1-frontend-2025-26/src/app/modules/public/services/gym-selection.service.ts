import { Injectable, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { ListGymsQueryDto } from '../../../api-services/gyms/gyms-api.models';

const STORAGE_KEY = 'selectedGym';

@Injectable({
  providedIn: 'root',
})
export class GymSelectionService {
  private readonly isBrowser = isPlatformBrowser(inject(PLATFORM_ID));

  selectGym(gym: ListGymsQueryDto): void {
    if (!this.isBrowser) return;
    localStorage.setItem(STORAGE_KEY, JSON.stringify(gym));
  }

  getSelectedGym(): ListGymsQueryDto | null {
    if (!this.isBrowser) return null;
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as ListGymsQueryDto;
    } catch {
      return null;
    }
  }

  clearSelection(): void {
    if (!this.isBrowser) return;
    localStorage.removeItem(STORAGE_KEY);
  }
}
