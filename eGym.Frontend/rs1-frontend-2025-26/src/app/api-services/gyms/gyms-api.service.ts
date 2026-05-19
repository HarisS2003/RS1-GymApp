import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  GetGymByIdQueryDto,
  ListGymsRequest,
  ListGymsResponse,
} from './gyms-api.models';

@Injectable({
  providedIn: 'root',
})
export class GymsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Gyms`;
  private http = inject(HttpClient);

  list(request?: ListGymsRequest): Observable<ListGymsResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListGymsResponse>(this.baseUrl, { params });
  }

  getById(id: number): Observable<GetGymByIdQueryDto> {
    return this.http.get<GetGymByIdQueryDto>(`${this.baseUrl}/${id}`);
  }
}
