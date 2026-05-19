import { Injectable } from '@angular/core';
import { ListGymsQueryDto } from '../../../api-services/gyms/gyms-api.models';

const STORAGE_KEY = 'selectedGym';

@Injectable({
  providedIn: 'root',
})
export class GymSelectionService {
  selectGym(gym: ListGymsQueryDto): void {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(gym));
  }

  getSelectedGym(): ListGymsQueryDto | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return null;
    try {
      return JSON.parse(raw) as ListGymsQueryDto;
    } catch {
      return null;
    }
  }

  clearSelection(): void {
    localStorage.removeItem(STORAGE_KEY);
  }
}
