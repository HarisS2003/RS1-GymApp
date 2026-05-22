using Market.Application.Modules.Identity.UserMemberships.Commands.Purchase;
using Market.Application.Modules.Identity.UserMemberships.Queries.GetMyActive;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserMembershipsController(ISender sender) : ControllerBase
{
    [HttpPost("purchase")]
    public async Task<ActionResult<PurchaseMembershipPlanResultDto>> Purchase(
        PurchaseMembershipPlanCommand command,
        CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyActive(CancellationToken ct)
    {
        var dto = await sender.Send(new GetMyActiveUserMembershipQuery(), ct);
        return new JsonResult(dto) { StatusCode = StatusCodes.Status200OK };
    }
}
