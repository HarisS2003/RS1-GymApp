namespace Market.Domain.Entities;

public class MembershipEvent
{
    public int Id { get; set; }
    public int? UserMembershipId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }

    public UserMembershipEntity? UserMembership { get; set; }
}
