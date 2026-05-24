import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApproveTrainingRequestResultDto,
  CreateTrainingRequestCommand,
  CreateTrainingRequestResultDto,
  ListTrainerTrainingRequestQueryDto,
  ListTrainingRequestQueryDto,
  TrainerAvailableSlotDto,
} from './training-requests-api.models';

@Injectable({
  providedIn: 'root',
})
export class TrainingRequestsApiService {
  private readonly baseUrl = `${environment.apiUrl}/TrainingRequests`;
  private http = inject(HttpClient);

  getAvailableSlots(trainerId: number, date: string): Observable<TrainerAvailableSlotDto[]> {
    const params = new HttpParams().set('trainerId', trainerId).set('date', date);
    return this.http.get<TrainerAvailableSlotDto[]>(`${this.baseUrl}/available-slots`, { params });
  }

  create(payload: CreateTrainingRequestCommand): Observable<CreateTrainingRequestResultDto> {
    return this.http.post<CreateTrainingRequestResultDto>(this.baseUrl, payload);
  }

  listMy(): Observable<ListTrainingRequestQueryDto[]> {
    return this.http.get<ListTrainingRequestQueryDto[]>(`${this.baseUrl}/my`);
  }

  listForTrainer(status?: number): Observable<ListTrainerTrainingRequestQueryDto[]> {
    let params = new HttpParams();
    if (status != null) params = params.set('status', status);
    return this.http.get<ListTrainerTrainingRequestQueryDto[]>(`${this.baseUrl}/trainer`, { params });
  }

  approve(id: number): Observable<ApproveTrainingRequestResultDto> {
    return this.http.post<ApproveTrainingRequestResultDto>(`${this.baseUrl}/${id}/approve`, {});
  }

  reject(id: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/reject`, {});
  }
}
