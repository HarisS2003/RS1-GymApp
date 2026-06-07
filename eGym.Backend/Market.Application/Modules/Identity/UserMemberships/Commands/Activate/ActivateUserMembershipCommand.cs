namespace Market.Application.Modules.Identity.UserMemberships.Commands.Activate;

public sealed class ActivateUserMembershipCommand : IRequest<Unit>
{
    public int UserMembershipId { get; set; }
    public string? Reason { get; set; }
}
