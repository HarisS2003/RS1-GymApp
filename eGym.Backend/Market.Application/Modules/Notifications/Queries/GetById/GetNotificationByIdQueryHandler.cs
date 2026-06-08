namespace Market.Application.Modules.Notifications.Queries.GetById;

public sealed class GetNotificationByIdQueryHandler(IAppDbContext ctx)
    : IRequestHandler<GetNotificationByIdQuery, GetNotificationByIdQueryDto>
{
    public async Task<GetNotificationByIdQueryDto> Handle(GetNotificationByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Notifications.AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new GetNotificationByIdQueryDto
            {
                Id = x.Id,
                UserId = x.UserId,
                Title = x.Title,
                Message = x.Message,
                Type = x.Type,
                IsRead = x.IsRead
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Notification (ID={request.Id}) nije pronađen.");
    }
}
