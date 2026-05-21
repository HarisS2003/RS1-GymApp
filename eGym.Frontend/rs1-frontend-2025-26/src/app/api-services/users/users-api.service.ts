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

  getById(id: number): Observable<GetUserByIdQueryDto> {
    return this.http.get<GetUserByIdQueryDto>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateUserCommand): Observable<number> {
    return this.http.post<number>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateUserCommand): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
