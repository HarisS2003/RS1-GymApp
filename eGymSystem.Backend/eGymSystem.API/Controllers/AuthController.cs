namespace eGymSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Scaffold placeholder.");
    }
}
