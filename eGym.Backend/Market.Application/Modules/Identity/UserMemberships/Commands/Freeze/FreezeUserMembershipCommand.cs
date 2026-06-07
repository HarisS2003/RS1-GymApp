namespace Market.Application.Modules.Identity.UserMemberships.Commands.Freeze;

public sealed class FreezeUserMembershipCommand : IRequest<Unit>
{
    public int UserMembershipId { get; set; }
    public string? Reason { get; set; }
}
