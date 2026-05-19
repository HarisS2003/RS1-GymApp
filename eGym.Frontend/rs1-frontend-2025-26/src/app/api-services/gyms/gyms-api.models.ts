import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListGymsRequest extends BasePagedQuery {
  search?: string | null;
  city?: string | null;
}

/** Matches ListGymsQueryDto.cs */
export interface ListGymsQueryDto {
  id: number;
  name: string;
  address: string;
  city: string;
}

export type ListGymsResponse = PageResult<ListGymsQueryDto>;

export interface GetGymByIdQueryDto {
  id: number;
  name: string;
  address: string;
  city: string;
}
