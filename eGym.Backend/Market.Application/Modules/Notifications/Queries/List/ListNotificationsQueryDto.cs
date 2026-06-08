namespace Market.Application.Modules.Notifications.Queries.List;

public sealed class ListNotificationsQueryDto
{
    public required int Id { get; init; }
    public required int UserId { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required string Type { get; init; }
    public required bool IsRead { get; init; }
}
