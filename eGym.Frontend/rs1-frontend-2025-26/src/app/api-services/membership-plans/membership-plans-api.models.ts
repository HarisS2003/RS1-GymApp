import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListMembershipPlansRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
}

export interface ListMembershipPlansQueryDto {
  id: number;
  name: string;
  durationDays: number;
  price: number;
  discountPercentage: number;
  gymId: number;
}

export type ListMembershipPlansResponse = PageResult<ListMembershipPlansQueryDto>;
