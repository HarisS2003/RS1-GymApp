import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateMembershipPlanCommand,
  GetMembershipPlanByIdQueryDto,
  ListMembershipPlansRequest,
  ListMembershipPlansResponse,
  UpdateMembershipPlanCommand,
} from './membership-plans-api.models';

@Injectable({
  providedIn: 'root',
})
export class MembershipPlansApiService {
  private readonly baseUrl = `${environment.apiUrl}/MembershipPlans`;
  private http = inject(HttpClient);

  list(request?: ListMembershipPlansRequest): Observable<ListMembershipPlansResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListMembershipPlansResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetMembershipPlanByIdQueryDto> {
    return this.http.get<GetMembershipPlanByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateMembershipPlanCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateMembershipPlanCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
