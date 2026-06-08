namespace Market.Application.Modules.Notifications.Commands.Create;

public sealed class CreateNotificationCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateNotificationCommand, int>
{
    public async Task<int> Handle(CreateNotificationCommand request, CancellationToken ct)
    {
        if (request.UserId <= 0) throw new ValidationException("UserId is required.");

        var title = request.Title.Trim();
        if (string.IsNullOrWhiteSpace(title)) throw new ValidationException("Title is required.");
        if (title.Length > 200) throw new ValidationException("Title must be at most 200 characters.");

        var message = request.Message.Trim();
        if (string.IsNullOrWhiteSpace(message)) throw new ValidationException("Message is required.");
        if (message.Length > 2000) throw new ValidationException("Message must be at most 2000 characters.");

        var type = request.Type.Trim();
        if (string.IsNullOrWhiteSpace(type)) throw new ValidationException("Type is required.");
        if (type.Length > 100) throw new ValidationException("Type must be at most 100 characters.");

        if (!await ctx.Users.AnyAsync(x => x.Id == request.UserId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid UserId.");

        var notification = new NotificationEntity
        {
            UserId = request.UserId,
            Title = title,
            Message = message,
            Type = type,
            IsRead = request.IsRead
        };

        ctx.Notifications.Add(notification);
        await ctx.SaveChangesAsync(ct);
        return notification.Id;
    }
}
