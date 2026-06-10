using Market.Application.Modules.Catalog.Trainers.Commands.Create;
using Market.Application.Modules.Catalog.Trainers.Commands.Delete;
using Market.Application.Modules.Catalog.Trainers.Commands.Update;
using Market.Application.Modules.Catalog.Trainers.Queries.GetById;
using Market.Application.Modules.Catalog.Trainers.Queries.List;
using Microsoft.AspNetCore.Authorization;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainersController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<string>> Create(CreateTrainerCommand command, CancellationToken ct)
    {
        var publicId = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { publicId }, new { publicId });
    }

    [HttpPut("{publicId}")]
    [Authorize]
    public async Task Update(string publicId, UpdateTrainerCommand command, CancellationToken ct)
    {
        command.PublicId = publicId;
        await sender.Send(command, ct);
    }

    [HttpDelete("{publicId}")]
    [Authorize]
    public async Task Delete(string publicId, CancellationToken ct)
    {
        await sender.Send(new DeleteTrainerCommand { PublicId = publicId }, ct);
    }

    [HttpGet("{publicId}")]
    [Authorize]
    public async Task<GetTrainerByIdQueryDto> GetById(string publicId, CancellationToken ct)
    {
        return await sender.Send(new GetTrainerByIdQuery { PublicId = publicId }, ct);
    }

    [HttpGet]
    [Authorize]
    public async Task<PageResult<ListTrainersQueryDto>> List([FromQuery] ListTrainersQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
