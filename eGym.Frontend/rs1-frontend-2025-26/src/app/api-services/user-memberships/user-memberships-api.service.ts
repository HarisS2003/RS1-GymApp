import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
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
}
