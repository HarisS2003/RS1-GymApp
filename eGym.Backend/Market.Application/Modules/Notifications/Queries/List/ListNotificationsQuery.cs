namespace Market.Application.Modules.Notifications.Queries.List;

public sealed class ListNotificationsQuery : BasePagedQuery<ListNotificationsQueryDto>
{
    public int? UserId { get; init; }
    public string? Type { get; init; }
    public bool? IsRead { get; init; }
    public string? Search { get; init; }
}
