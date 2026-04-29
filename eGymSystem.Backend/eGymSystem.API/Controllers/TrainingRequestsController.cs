namespace eGymSystem.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class TrainingRequestsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, "Scaffold placeholder.");
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new { id });
    }

    [HttpGet]
    public IActionResult List()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, Array.Empty<object>());
    }
}
