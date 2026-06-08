namespace Market.Application.Modules.Identity.UserMemberships.Commands.Freeze;



public sealed class FreezeUserMembershipCommand : IRequest<Unit>

{

    public string PublicId { get; set; } = string.Empty;

    public string? Reason { get; set; }

}

