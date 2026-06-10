using Market.Application.Modules.Catalog.Gyms.Commands.Create;
using Market.Application.Modules.Catalog.Gyms.Commands.Delete;
using Market.Application.Modules.Catalog.Gyms.Commands.Update;
using Market.Application.Modules.Catalog.Gyms.Queries.GetById;
using Market.Application.Modules.Catalog.Gyms.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class GymsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateGymCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateGymCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteGymCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetGymByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var gym = await sender.Send(new GetGymByIdQuery { Id = id }, ct);
        return gym;
    }

    [HttpGet]
    public async Task<PageResult<ListGymsQueryDto>> List([FromQuery] ListGymsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
