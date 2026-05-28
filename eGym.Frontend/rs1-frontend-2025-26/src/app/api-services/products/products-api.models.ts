import { PageResult } from '../../core/models/paging/page-result';
import { BasePagedQuery } from '../../core/models/paging/base-paged-query';

export class ListProductsRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
  size?: string | null;
  categoryName?: string | null;
}

export interface ProductVariantQueryDto {
  id: number;
  productId: number;
  size: string;
  color: string;
  price: number;
  stockQuantity: number;
}

export interface ProductVariantCommandDto {
  id?: number | null;
  size: string;
  color: string;
  price: number;
  stockQuantity: number;
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
  productVariants: ProductVariantQueryDto[];
}

export type ListProductsResponse = PageResult<ListProductsQueryDto>;

export interface CreateProductCommand {
  name: string;
  categoryName: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  gymId: number;
  variants: ProductVariantCommandDto[];
}

export interface UpdateProductCommand {
  name: string;
  categoryName: string;
  description?: string | null;
  price: number;
  stockQuantity: number;
  gymId: number;
  variants: ProductVariantCommandDto[];
}
