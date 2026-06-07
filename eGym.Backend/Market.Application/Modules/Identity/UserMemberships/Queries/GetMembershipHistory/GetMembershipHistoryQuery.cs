namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;

public sealed class GetMembershipHistoryQuery : IRequest<GetMembershipHistoryQueryDto>
{
    public int UserMembershipId { get; set; }
    public DateTime AsOfDate { get; set; } = DateTime.UtcNow;
}
