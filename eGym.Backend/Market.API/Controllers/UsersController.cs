using Market.Application.Modules.Catalog.Users.Commands.Create;
using Market.Application.Modules.Catalog.Users.Commands.Delete;
using Market.Application.Modules.Catalog.Users.Commands.Update;
using Market.Application.Modules.Catalog.Users.Queries.GetById;
using Market.Application.Modules.Catalog.Users.Queries.List;
using Market.Application.Modules.Catalog.Users.Queries.ListWithMembership;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateUserCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateUserCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteUserCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetUserByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var user = await sender.Send(new GetUserByIdQuery { Id = id }, ct);
        return user;
    }

    [HttpGet]
    public async Task<PageResult<ListUsersQueryDto>> List([FromQuery] ListUsersQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    [HttpGet("with-memberships")]
    public async Task<PageResult<ListUsersWithMembershipQueryDto>> ListWithMemberships(
        [FromQuery] ListUsersWithMembershipQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
