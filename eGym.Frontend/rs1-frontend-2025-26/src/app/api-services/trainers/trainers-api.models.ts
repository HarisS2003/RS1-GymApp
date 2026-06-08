import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListTrainersRequest extends BasePagedQuery {
  search?: string | null;
  gymId?: number | null;
  userPublicId?: string | null;
  minExperienceYears?: number | null;
}

export interface ListTrainersQueryDto {
  publicId: string;
  userPublicId: string;
  gymId: number;
  bio: string;
  experienceYears: number;
}

export type ListTrainersResponse = PageResult<ListTrainersQueryDto>;

/** Matches CreateTrainerCommand.cs */
export interface CreateTrainerCommand {
  userPublicId: string;
  gymId: number;
  bio: string;
  experienceYears: number;
}

/** Matches UpdateTrainerCommand.cs */
export interface UpdateTrainerCommand {
  userPublicId: string;
  gymId: number;
  bio: string;
  experienceYears: number;
}
