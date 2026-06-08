namespace Market.Application.Modules.Catalog.Users.Commands.Delete;

public sealed class DeleteUserCommand : IRequest<Unit>
{
    public required string PublicId { get; set; }
}
