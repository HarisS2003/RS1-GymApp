import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { ListTrainingsRequest, ListTrainingsResponse } from './trainings-api.models';

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
}
