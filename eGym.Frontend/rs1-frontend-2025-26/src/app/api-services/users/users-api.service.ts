import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateUserCommand,
  GetUserByIdQueryDto,
  ListUsersRequest,
  ListUsersResponse,
  ListUsersWithMembershipRequest,
  ListUsersWithMembershipResponse,
  UpdateUserCommand,
} from './users-api.models';

@Injectable({
  providedIn: 'root',
})
export class UsersApiService {
  private readonly baseUrl = `${environment.apiUrl}/Users`;
  private http = inject(HttpClient);

  list(request?: ListUsersRequest): Observable<ListUsersResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListUsersResponse>(this.baseUrl, { params });
  }

  listWithMemberships(
    request?: ListUsersWithMembershipRequest,
  ): Observable<ListUsersWithMembershipResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListUsersWithMembershipResponse>(`${this.baseUrl}/with-memberships`, {
      params,
    });
  }

  getCurrent(): Observable<GetUserByIdQueryDto> {
    return this.http.get<GetUserByIdQueryDto>(`${this.baseUrl}/me`);
  }

  getById(publicId: string): Observable<GetUserByIdQueryDto> {
    return this.http.get<GetUserByIdQueryDto>(`${this.baseUrl}/${publicId}`);
  }

  create(payload: CreateUserCommand): Observable<string> {
    return this.http.post<string>(this.baseUrl, payload);
  }

  update(publicId: string, payload: UpdateUserCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${publicId}`, payload);
  }

  delete(publicId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${publicId}`);
  }
}

