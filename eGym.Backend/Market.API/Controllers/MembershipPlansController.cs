using Market.Application.Modules.Catalog.MembershipPlans.Commands.Create;
using Market.Application.Modules.Catalog.MembershipPlans.Commands.Delete;
using Market.Application.Modules.Catalog.MembershipPlans.Commands.Update;
using Market.Application.Modules.Catalog.MembershipPlans.Queries.GetById;
using Market.Application.Modules.Catalog.MembershipPlans.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MembershipPlansController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateMembershipPlanCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateMembershipPlanCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteMembershipPlanCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetMembershipPlanByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var dto = await sender.Send(new GetMembershipPlanByIdQuery { Id = id }, ct);
        return dto;
    }

    [HttpGet]
    public async Task<PageResult<ListMembershipPlansQueryDto>> List([FromQuery] ListMembershipPlansQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }
}
