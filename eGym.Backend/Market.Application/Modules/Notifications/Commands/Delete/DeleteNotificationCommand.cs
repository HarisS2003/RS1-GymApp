namespace Market.Application.Modules.Notifications.Commands.Delete;

public sealed class DeleteNotificationCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
