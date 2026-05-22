import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

export class ListProductsRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
}

export interface ListProductsQueryDto {
  id: number;
  name: string;
  categoryName: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  gymId: number;
  isEnabled: boolean;
}

export interface GetProductByIdQueryDto {
  id: number;
  name: string;
  categoryName: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  gymId: number;
  isEnabled: boolean;
}

export type ListProductsResponse = PageResult<ListProductsQueryDto>;

export interface CreateProductCommand {
  name: string;
  categoryName: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  gymId: number;
}

export interface UpdateProductCommand {
  name: string;
  categoryName: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  gymId: number;
}
