namespace Market.Application.Modules.Notifications.Queries.GetById;

public sealed class GetNotificationByIdQuery : IRequest<GetNotificationByIdQueryDto>
{
    public int Id { get; set; }
}
