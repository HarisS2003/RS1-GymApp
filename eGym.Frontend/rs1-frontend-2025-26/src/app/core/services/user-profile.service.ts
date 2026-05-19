import { Injectable, inject, signal, computed } from '@angular/core';
import { catchError, map, Observable, of, switchMap, tap } from 'rxjs';
import { UsersApiService } from '../../api-services/users/users-api.service';
import { GymsApiService } from '../../api-services/gyms/gyms-api.service';
import { AuthFacadeService } from './auth/auth-facade.service';
import {
  ADMIN_ROLE_ID,
  MEMBER_ROLE_ID,
  TRAINER_ROLE_ID,
} from '../../modules/auth/constants/auth.constants';

export interface UserProfileView {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  roleId: number;
  gymId: number;
  gymName: string;
  fullName: string;
}

@Injectable({ providedIn: 'root' })
export class UserProfileService {
  private usersApi = inject(UsersApiService);
  private gymsApi = inject(GymsApiService);
  private auth = inject(AuthFacadeService);

  private _profile = signal<UserProfileView | null>(null);
  profile = this._profile.asReadonly();

  isTrainer = computed(() => this._profile()?.roleId === TRAINER_ROLE_ID);
  isMember = computed(() => this._profile()?.roleId === MEMBER_ROLE_ID);
  isAdminRole = computed(() => this._profile()?.roleId === ADMIN_ROLE_ID);

  loadProfile(): Observable<UserProfileView | null> {
    const userId = this.auth.currentUser()?.userId;
    if (!userId) {
      this._profile.set(null);
      return of(null);
    }

    return this.usersApi.getById(userId).pipe(
      switchMap((user) =>
        this.gymsApi.getById(user.gymId).pipe(
          map((gym) => ({
            id: user.id,
            firstName: user.firstName,
            lastName: user.lastName,
            email: user.email,
            roleId: user.roleId,
            gymId: user.gymId,
            gymName: gym.name,
            fullName: `${user.firstName} ${user.lastName}`.trim(),
          })),
          catchError(() =>
            of({
              id: user.id,
              firstName: user.firstName,
              lastName: user.lastName,
              email: user.email,
              roleId: user.roleId,
              gymId: user.gymId,
              gymName: '',
              fullName: `${user.firstName} ${user.lastName}`.trim(),
            }),
          ),
        ),
      ),
      tap((view) => this._profile.set(view)),
      catchError(() => {
        this._profile.set(null);
        return of(null);
      }),
    );
  }

  clear(): void {
    this._profile.set(null);
  }
}
