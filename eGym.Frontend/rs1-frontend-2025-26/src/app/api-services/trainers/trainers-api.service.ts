import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { buildHttpParams } from '../../core/models/build-http-params';
import { ListTrainersRequest, ListTrainersResponse } from './trainers-api.models';

@Injectable({
  providedIn: 'root',
})
export class TrainersApiService {
  private readonly baseUrl = `${environment.apiUrl}/Trainers`;
  private http = inject(HttpClient);

  list(request?: ListTrainersRequest): Observable<ListTrainersResponse> {
    const params = request ? buildHttpParams(request as any) : undefined;
    return this.http.get<ListTrainersResponse>(this.baseUrl, { params });
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
