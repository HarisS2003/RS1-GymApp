import { BasePagedQuery } from '../../core/models/paging/base-paged-query';
import { PageResult } from '../../core/models/paging/page-result';

export class ListTrainingsRequest extends BasePagedQuery {
  trainerId?: number | null;
  type?: number | null;
  dateFrom?: string | null;
  dateTo?: string | null;
}

export interface ListTrainingsQueryDto {
  id: number;
  trainerId: number;
  trainerName: string;
  type: number;
  description?: string | null;
  date: string;
  startTime: string;
  capacity: number;
  participantsCount: number;
}

export interface CreateTrainingCommand {
  trainerId: number;
  type: number;
  description?: string | null;
  date: string;
  startTime: string;
  capacity: number;
}

export type ListTrainingsResponse = PageResult<ListTrainingsQueryDto>;
