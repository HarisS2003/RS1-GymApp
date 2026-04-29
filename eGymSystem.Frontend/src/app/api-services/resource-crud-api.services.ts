import { Injectable } from '@angular/core';
import { BaseCrudApiService } from './base-crud-api.service';
import { apiConfig } from '../core/config/api.config';
import { CrudEntity } from '../core/models/crud.models';

type UnknownPayload = Record<string, unknown>;

interface ResourceEntity extends CrudEntity {
  name?: string;
}

@Injectable({ providedIn: 'root' })
export class UsersApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/users`;
}

@Injectable({ providedIn: 'root' })
export class TrainersApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/trainers`;
}

@Injectable({ providedIn: 'root' })
export class TrainingsApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/trainings`;
}

@Injectable({ providedIn: 'root' })
export class TrainingRequestsApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/training-requests`;
}

@Injectable({ providedIn: 'root' })
export class ProductsApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/products`;
}

@Injectable({ providedIn: 'root' })
export class OrdersApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/orders`;
}

@Injectable({ providedIn: 'root' })
export class MembershipsApiService extends BaseCrudApiService<ResourceEntity, UnknownPayload, UnknownPayload> {
  protected readonly endpoint = `${apiConfig.baseUrl}/memberships`;
}
