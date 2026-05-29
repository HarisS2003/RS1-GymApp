using Microsoft.AspNetCore.RateLimiting;

namespace Market.API.Controllers;

[ApiController]
[Route("api/health")]
[AllowAnonymous]
[DisableRateLimiting]
public sealed class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "Healthy",
            timestamp = DateTimeOffset.UtcNow,
        });
    }
}
