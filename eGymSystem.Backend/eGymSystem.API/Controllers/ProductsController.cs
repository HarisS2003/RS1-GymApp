namespace eGymSystem.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class ProductsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpGet]
    [AllowAnonymous]
    public IActionResult List() => StatusCode(StatusCodes.Status501NotImplemented, Array.Empty<object>());
}
