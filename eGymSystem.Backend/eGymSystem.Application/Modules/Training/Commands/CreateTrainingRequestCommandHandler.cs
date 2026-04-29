using System.Threading;
using eGymSystem.Application.Abstractions.Messaging;

namespace eGymSystem.Application.Modules.Training.Commands;

internal sealed class CreateTrainingRequestCommandHandler : ICommandHandler<CreateTrainingRequestCommand, CreateTrainingRequestResult>
{
    private static int _lastId = 1000;

    public Task<CreateTrainingRequestResult> Handle(CreateTrainingRequestCommand command, CancellationToken cancellationToken)
    {
        var nextId = Interlocked.Increment(ref _lastId);
        var response = new CreateTrainingRequestResult(nextId, "Pending");
        return Task.FromResult(response);
    }
}
