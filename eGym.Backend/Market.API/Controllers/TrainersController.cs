using Market.Application.Modules.Catalog.Trainers.Commands.Create;
using Market.Application.Modules.Catalog.Trainers.Commands.Delete;
using Market.Application.Modules.Catalog.Trainers.Commands.Update;
using Market.Application.Modules.Catalog.Trainers.Queries.GetById;
using Market.Application.Modules.Catalog.Trainers.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class TrainersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTrainerCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateTrainerCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteTrainerCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetTrainerByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var trainer = await sender.Send(new GetTrainerByIdQuery { Id = id }, ct);
        return trainer;
    }

    [HttpGet]
    public async Task<PageResult<ListTrainersQueryDto>> List([FromQuery] ListTrainersQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
