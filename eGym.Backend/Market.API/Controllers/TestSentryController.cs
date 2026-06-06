using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Market.API.Controllers;

[ApiController]
[Route("api/test-sentry")]
[AllowAnonymous]
[DisableRateLimiting]
public sealed class TestSentryController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        throw new Exception("Testna Sentry greska sa eGym API-ja");
    }
}
