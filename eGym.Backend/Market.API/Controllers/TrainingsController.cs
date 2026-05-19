using Market.Application.Modules.Catalog.Trainings.Commands.Create;
using Market.Application.Modules.Catalog.Trainings.Commands.Delete;
using Market.Application.Modules.Catalog.Trainings.Commands.Update;
using Market.Application.Modules.Catalog.Trainings.Queries.GetById;
using Market.Application.Modules.Catalog.Trainings.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class TrainingsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateTrainingCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateTrainingCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteTrainingCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetTrainingByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetTrainingByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    public async Task<PageResult<ListTrainingsQueryDto>> List([FromQuery] ListTrainingsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
