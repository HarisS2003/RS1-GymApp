import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import {
  CreateTrainingCommand,
  ListTrainingsQueryDto,
  ListTrainingsRequest,
  ListTrainingsResponse,
} from './trainings-api.models';

@Injectable({
  providedIn: 'root',
})
export class TrainingsApiService {
  private readonly baseUrl = `${environment.apiUrl}/Trainings`;
  private http = inject(HttpClient);

  list(request?: ListTrainingsRequest): Observable<ListTrainingsResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListTrainingsResponse>(this.baseUrl, { params });
  }

  listMy(): Observable<ListTrainingsQueryDto[]> {
    return this.http.get<ListTrainingsQueryDto[]>(`${this.baseUrl}/my`);
  }

  create(payload: CreateTrainingCommand): Observable<{ id: number }> {
    return this.http.post<{ id: number }>(this.baseUrl, payload);
  }

  join(id: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/join`, {});
  }
}
