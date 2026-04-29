import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CrudEntity } from '../core/models/crud.models';

export abstract class BaseCrudApiService<T extends CrudEntity, TCreate, TUpdate> {
  protected readonly http = inject(HttpClient);
  protected abstract readonly endpoint: string;

  list(): Observable<T[]> {
    return this.http.get<T[]>(this.endpoint);
  }

  getById(id: number): Observable<T> {
    return this.http.get<T>(`${this.endpoint}/${id}`);
  }

  create(payload: TCreate): Observable<unknown> {
    return this.http.post(this.endpoint, payload);
  }

  update(id: number, payload: TUpdate): Observable<unknown> {
    return this.http.put(`${this.endpoint}/${id}`, payload);
  }

  delete(id: number): Observable<unknown> {
    return this.http.delete(`${this.endpoint}/${id}`);
  }
}
