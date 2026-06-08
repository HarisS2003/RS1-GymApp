namespace Market.Application.Modules.Identity.UserMemberships.Commands.Activate;



public sealed class ActivateUserMembershipCommand : IRequest<Unit>

{

    public string PublicId { get; set; } = string.Empty;

    public string? Reason { get; set; }

}

