namespace Market.Application.Modules.Notifications.Commands.Update;

public sealed class UpdateNotificationCommand : IRequest<Unit>
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}
