import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListUsersRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
  roleId?: number | null;
}

export interface ListUsersQueryDto {
  publicId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  roleId: number;
  gymId: number;
}

export type ListUsersResponse = PageResult<ListUsersQueryDto>;

export class ListUsersWithMembershipRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
  roleId?: number | null;
}

export interface ListUsersWithMembershipQueryDto {
  publicId: string;
  firstName: string;
  lastName: string;
  email: string;
  membershipPublicId: string | null;
  currentMembershipName: string | null;
  membershipStatus: string;
}

export type ListUsersWithMembershipResponse = PageResult<ListUsersWithMembershipQueryDto>;

export interface GetUserByIdQueryDto {
  publicId: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  roleId: number;
  gymId: number;
}

/** Matches CreateUserCommand.cs */
export interface CreateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  password: string;
  roleId: number;
  gymId: number;
}

/** Matches UpdateUserCommand.cs */
export interface UpdateUserCommand {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  password?: string | null;
  roleId: number;
  gymId: number;
}

