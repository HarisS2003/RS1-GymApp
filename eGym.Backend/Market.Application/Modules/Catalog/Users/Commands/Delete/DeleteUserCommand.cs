namespace Market.Application.Modules.Catalog.Users.Commands.Delete;

public class DeleteUserCommand : IRequest<Unit>
{
    public required int Id { get; set; }
}
