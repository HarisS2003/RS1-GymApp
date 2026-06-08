namespace Market.Application.Modules.Notifications.Queries.GetById;

public sealed class GetNotificationByIdQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required string Type { get; init; }
    public required bool IsRead { get; init; }
}
