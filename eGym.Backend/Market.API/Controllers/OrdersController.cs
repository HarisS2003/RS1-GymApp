using Market.Application.Modules.Catalog.Orders.Commands.Create;
using Market.Application.Modules.Catalog.Orders.Commands.Delete;
using Market.Application.Modules.Catalog.Orders.Commands.Update;
using Market.Application.Modules.Catalog.Orders.Queries.GetById;
using Market.Application.Modules.Catalog.Orders.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateOrderCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateOrderCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteOrderCommand { Id = id }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetOrderByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetOrderByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    public async Task<PageResult<ListOrdersQueryDto>> List([FromQuery] ListOrdersQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }
}
