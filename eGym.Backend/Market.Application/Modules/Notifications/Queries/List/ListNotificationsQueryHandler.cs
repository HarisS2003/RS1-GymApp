namespace Market.Application.Modules.Notifications.Queries.List;

public sealed class ListNotificationsQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListNotificationsQuery, PageResult<ListNotificationsQueryDto>>
{
    public async Task<PageResult<ListNotificationsQueryDto>> Handle(ListNotificationsQuery request, CancellationToken ct)
    {
        var q = ctx.Notifications.AsNoTracking().Where(x => !x.IsDeleted);

        if (request.UserId is int userId)
            q = q.Where(x => x.UserId == userId);

        if (!string.IsNullOrWhiteSpace(request.Type))
        {
            var type = request.Type.Trim().ToLower();
            q = q.Where(x => x.Type.ToLower() == type);
        }

        if (request.IsRead is bool isRead)
            q = q.Where(x => x.IsRead == isRead);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim().ToLower();
            q = q.Where(x => x.Title.ToLower().Contains(term) || x.Message.ToLower().Contains(term));
        }

        var projectedQuery = q.Select(x => new ListNotificationsQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            Title = x.Title,
            Message = x.Message,
            Type = x.Type,
            IsRead = x.IsRead
        });

        return await PageResult<ListNotificationsQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
