import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  GetMembershipHistoryQueryDto,
  GetMyActiveUserMembershipQueryDto,
  ListMyMembershipPurchaseHistoryQueryDto,
  PurchaseMembershipPlanCommand,
  PurchaseMembershipPlanResultDto,
} from './user-memberships-api.models';

@Injectable({
  providedIn: 'root',
})
export class UserMembershipsApiService {
  private readonly baseUrl = `${environment.apiUrl}/UserMemberships`;
  private http = inject(HttpClient);

  getMyActive(): Observable<GetMyActiveUserMembershipQueryDto | null> {
    return this.http.get<GetMyActiveUserMembershipQueryDto | null>(`${this.baseUrl}/my`).pipe(
      map((dto) => (dto && dto.userMembershipId > 0 ? dto : null)),
    );
  }

  listMyHistory(): Observable<ListMyMembershipPurchaseHistoryQueryDto[]> {
    return this.http.get<ListMyMembershipPurchaseHistoryQueryDto[]>(`${this.baseUrl}/my/history`);
  }

  purchase(
    payload: PurchaseMembershipPlanCommand,
  ): Observable<PurchaseMembershipPlanResultDto> {
    return this.http.post<PurchaseMembershipPlanResultDto>(
      `${this.baseUrl}/purchase`,
      payload,
    );
  }

  getHistory(
    userMembershipId: number,
    asOfDate?: string,
  ): Observable<GetMembershipHistoryQueryDto> {
    const params = asOfDate ? { asOfDate } : undefined;
    return this.http.get<GetMembershipHistoryQueryDto>(
      `${this.baseUrl}/${userMembershipId}/history`,
      { params },
    );
  }

  freeze(userMembershipId: number, reason?: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${userMembershipId}/freeze`, { reason });
  }

  activate(userMembershipId: number, reason?: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${userMembershipId}/activate`, { reason });
  }
}
