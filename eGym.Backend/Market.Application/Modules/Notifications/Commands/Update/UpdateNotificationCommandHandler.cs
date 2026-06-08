namespace Market.Application.Modules.Notifications.Commands.Update;

public sealed class UpdateNotificationCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateNotificationCommand, Unit>
{
    public async Task<Unit> Handle(UpdateNotificationCommand request, CancellationToken ct)
    {
        var entity = await ctx.Notifications.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
        if (entity is null) throw new MarketNotFoundException($"Notification (ID={request.Id}) nije pronađen.");

        var title = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(title)) throw new ValidationException("Title is required.");
        if (title.Length > 200) throw new ValidationException("Title must be at most 200 characters.");

        var message = request.Message.Trim();
        if (string.IsNullOrWhiteSpace(message)) throw new ValidationException("Message is required.");
        if (message.Length > 2000) throw new ValidationException("Message must be at most 2000 characters.");

        var type = request.Type.Trim();
        if (string.IsNullOrWhiteSpace(type)) throw new ValidationException("Type is required.");
        if (type.Length > 100) throw new ValidationException("Type must be at most 100 characters.");

        entity.Title = title;
        entity.Message = message;
        entity.Type = type;
        entity.IsRead = request.IsRead;
        entity.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
