using Market.Application.Modules.Identity.UserMemberships.Commands.Purchase;
using Market.Application.Modules.Identity.UserMemberships.Queries.GetMyActive;
using Market.Application.Modules.Identity.UserMemberships.Queries.ListMyHistory;
using Microsoft.AspNetCore.Authorization;

namespace Market.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
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

    [HttpGet("my/history")]
    public async Task<IActionResult> ListMyHistory(CancellationToken ct)
    {
        var list = await sender.Send(new ListMyMembershipPurchaseHistoryQuery(), ct);
        return new JsonResult(list) { StatusCode = StatusCodes.Status200OK };
    }
}
