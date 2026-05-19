import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListUsersRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
  roleId?: number | null;
}

export interface ListUsersQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  roleId: number;
  gymId: number;
}

export type ListUsersResponse = PageResult<ListUsersQueryDto>;

export interface GetUserByIdQueryDto {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  roleId: number;
  gymId: number;
}

/** Matches CreateUserCommand.cs */
export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  roleId: number;
  gymId: number;
}
