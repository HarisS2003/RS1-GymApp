namespace Market.Application.Modules.Notifications.Commands.Delete;

public sealed class DeleteNotificationCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteNotificationCommand, Unit>
{
    public async Task<Unit> Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        var notification = await ctx.Notifications.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
        if (notification is null) throw new MarketNotFoundException("Notification nije pronađen.");

        notification.IsDeleted = true;
        notification.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
