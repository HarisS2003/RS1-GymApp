/** TrainingRequestStatus */
export const TRAINING_REQUEST_PENDING = 1;
export const TRAINING_REQUEST_APPROVED = 2;
export const TRAINING_REQUEST_REJECTED = 3;

export interface TrainerAvailableSlotDto {
  startTime: string;
  isAvailable: boolean;
}

export interface CreateTrainingRequestCommand {
  trainerId: number;
  date: string;
  startTime: string;
}

export interface CreateTrainingRequestResultDto {
  trainingRequestId: number;
  trainerId: number;
  date: string;
  startTime: string;
  status: number;
}

export interface ListTrainingRequestQueryDto {
  id: number;
  userId: number;
  trainerId: number;
  memberName: string;
  trainerName: string;
  date: string;
  startTime: string;
  status: number;
}

export interface ListTrainerTrainingRequestQueryDto {
  id: number;
  userId: number;
  trainerId: number;
  memberName: string;
  date: string;
  startTime: string;
  status: number;
}

export interface ApproveTrainingRequestResultDto {
  trainingRequestId: number;
  trainingId: number;
  status: number;
}
