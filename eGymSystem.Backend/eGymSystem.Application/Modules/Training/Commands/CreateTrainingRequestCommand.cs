using eGymSystem.Application.Abstractions.Messaging;

namespace eGymSystem.Application.Modules.Training.Commands;

public sealed record CreateTrainingRequestCommand(
    int UserId,
    int TrainerId,
    DateTime RequestedTimeUtc) : ICommand<CreateTrainingRequestResult>;

public sealed record CreateTrainingRequestResult(
    int RequestId,
    string Status);
