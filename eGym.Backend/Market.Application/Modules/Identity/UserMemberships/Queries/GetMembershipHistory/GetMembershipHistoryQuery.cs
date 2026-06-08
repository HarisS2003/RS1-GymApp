namespace Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;



public sealed class GetMembershipHistoryQuery : IRequest<GetMembershipHistoryQueryDto>

{

    public string PublicId { get; set; } = string.Empty;

    public DateTime AsOfDate { get; set; }

}

