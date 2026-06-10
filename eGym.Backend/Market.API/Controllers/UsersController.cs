using Market.Application.Modules.Catalog.Users.Commands.Create;

using Market.Application.Modules.Catalog.Users.Commands.Delete;

using Market.Application.Modules.Catalog.Users.Commands.Update;

using Market.Application.Modules.Catalog.Users.Queries.GetById;

using Market.Application.Modules.Catalog.Users.Queries.GetCurrent;

using Market.Application.Modules.Catalog.Users.Queries.List;

using Market.Application.Modules.Catalog.Users.Queries.ListWithMembership;

using Microsoft.AspNetCore.Authorization;



namespace Market.API.Controllers;



[ApiController]

[Route("api/[controller]")]

public class UsersController(ISender sender) : ControllerBase

{

    [HttpPost]

    [AllowAnonymous]

    public async Task<ActionResult<string>> Create(CreateUserCommand command, CancellationToken ct)

    {

        var publicId = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { publicId }, new { publicId });

    }



    [HttpPut("{publicId}")]

    [Authorize]

    public async Task Update(string publicId, UpdateUserCommand command, CancellationToken ct)

    {

        command.PublicId = publicId;

        await sender.Send(command, ct);

    }



    [HttpDelete("{publicId}")]

    [Authorize]

    public async Task Delete(string publicId, CancellationToken ct)

    {

        await sender.Send(new DeleteUserCommand { PublicId = publicId }, ct);

    }



    [HttpGet("me")]

    [Authorize]

    public async Task<GetUserByIdQueryDto> GetCurrent(CancellationToken ct)

    {

        return await sender.Send(new GetCurrentUserQuery(), ct);

    }



    [HttpGet("{publicId}")]

    [Authorize]

    public async Task<GetUserByIdQueryDto> GetById(string publicId, CancellationToken ct)

    {

        return await sender.Send(new GetUserByIdQuery { PublicId = publicId }, ct);

    }



    [HttpGet]

    [Authorize]

    public async Task<PageResult<ListUsersQueryDto>> List([FromQuery] ListUsersQuery query, CancellationToken ct)

    {

        return await sender.Send(query, ct);

    }



    [HttpGet("with-memberships")]

    [Authorize]

    public async Task<PageResult<ListUsersWithMembershipQueryDto>> ListWithMemberships(

        [FromQuery] ListUsersWithMembershipQuery query,

        CancellationToken ct)

    {

        return await sender.Send(query, ct);

    }

}

