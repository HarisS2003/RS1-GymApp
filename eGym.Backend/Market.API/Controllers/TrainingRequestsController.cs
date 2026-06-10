using Market.Application.Modules.Catalog.TrainingRequests.Commands.Approve;
using Market.Application.Modules.Catalog.TrainingRequests.Commands.Create;
using Market.Application.Modules.Catalog.TrainingRequests.Commands.Reject;
using Market.Application.Modules.Catalog.TrainingRequests.Queries.GetAvailableSlots;
using Market.Application.Modules.Catalog.TrainingRequests.Queries.ListForTrainer;
using Market.Application.Modules.Catalog.TrainingRequests.Queries.ListMy;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingRequestsController(ISender sender) : ControllerBase
{
    [HttpGet("available-slots")]
    public async Task<List<TrainerAvailableSlotDto>> GetAvailableSlots(
        [FromQuery] GetTrainerAvailableSlotsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpPost]
    public async Task<ActionResult<CreateTrainingRequestResultDto>> Create(
        CreateTrainingRequestCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<List<ListTrainingRequestQueryDto>> ListMy(CancellationToken ct)
    {
        return await sender.Send(new ListMyTrainingRequestsQuery(), ct);
    }

    [HttpGet("trainer")]
    public async Task<List<ListTrainerTrainingRequestQueryDto>> ListForTrainer(
        [FromQuery] ListTrainerTrainingRequestsQuery query,
        CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpPost("{id:int}/approve")]
    public async Task<ApproveTrainingRequestResultDto> Approve(int id, CancellationToken ct)
    {
        return await sender.Send(new ApproveTrainingRequestCommand { Id = id }, ct);
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        await sender.Send(new RejectTrainingRequestCommand { Id = id }, ct);
        return NoContent();
    }
}
