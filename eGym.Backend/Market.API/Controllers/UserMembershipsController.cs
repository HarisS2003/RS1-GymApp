using Market.Application.Modules.Identity.UserMemberships.Commands.Activate;
using Market.Application.Modules.Identity.UserMemberships.Commands.Freeze;
using Market.Application.Modules.Identity.UserMemberships.Commands.Purchase;
using Market.Application.Modules.Identity.UserMemberships.Queries.GetMembershipHistory;
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

    [HttpGet("{userMembershipId:int}/history")]
    public async Task<GetMembershipHistoryQueryDto> GetHistory(
        int userMembershipId,
        [FromQuery] DateTime? asOfDate,
        CancellationToken ct)
    {
        return await sender.Send(new GetMembershipHistoryQuery
        {
            UserMembershipId = userMembershipId,
            AsOfDate = asOfDate ?? DateTime.UtcNow,
        }, ct);
    }

    [HttpPost("{userMembershipId:int}/freeze")]
    public async Task Freeze(int userMembershipId, [FromBody] FreezeUserMembershipCommand? command, CancellationToken ct)
    {
        await sender.Send(new FreezeUserMembershipCommand
        {
            UserMembershipId = userMembershipId,
            Reason = command?.Reason,
        }, ct);
    }

    [HttpPost("{userMembershipId:int}/activate")]
    public async Task Activate(int userMembershipId, [FromBody] ActivateUserMembershipCommand? command, CancellationToken ct)
    {
        await sender.Send(new ActivateUserMembershipCommand
        {
            UserMembershipId = userMembershipId,
            Reason = command?.Reason,
        }, ct);
    }
}
