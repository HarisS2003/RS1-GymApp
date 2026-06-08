using System.Text.Json;

namespace Market.Application.Modules.Identity.UserMemberships.Services;

public static class MembershipEventRecorder
{
    public static void Append(
        IAppDbContext ctx,
        int userMembershipId,
        string eventType,
        object eventData,
        DateTime? createdAt = null)
    {
        ctx.MembershipEvents.Add(new MembershipEvent
        {
            UserMembershipId = userMembershipId,
            EventType = eventType,
            EventData = JsonSerializer.Serialize(eventData),
            CreatedAt = createdAt ?? DateTime.UtcNow,
        });
    }

    public static void AppendForMembership(
        IAppDbContext ctx,
        UserMembershipEntity membership,
        string eventType,
        object eventData,
        DateTime? createdAt = null)
    {
        ctx.MembershipEvents.Add(new MembershipEvent
        {
            UserMembership = membership,
            EventType = eventType,
            EventData = JsonSerializer.Serialize(eventData),
            CreatedAt = createdAt ?? DateTime.UtcNow,
        });
    }
}
