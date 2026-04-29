namespace eGymSystem.API.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class UsersController : ControllerBase
{
    [HttpPost]
    public IActionResult Create() => StatusCode(StatusCodes.Status501NotImplemented);

    [HttpPut("{id:int}")]
    public IActionResult Update(int id) => StatusCode(StatusCodes.Status501NotImplemented, new { id });

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) => StatusCode(StatusCodes.Status501NotImplemented, new { id });

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id) => StatusCode(StatusCodes.Status501NotImplemented, new { id });
}
