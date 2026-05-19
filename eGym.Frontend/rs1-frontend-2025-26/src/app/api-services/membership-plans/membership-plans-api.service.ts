import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  ListMembershipPlansRequest,
  ListMembershipPlansResponse,
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
}
