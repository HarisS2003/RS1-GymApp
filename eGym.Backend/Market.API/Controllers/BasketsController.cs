using Market.Application.Modules.Catalog.Baskets.Commands.AddItem;
using Market.Application.Modules.Catalog.Baskets.Commands.Create;
using Market.Application.Modules.Catalog.Baskets.Commands.Delete;
using Market.Application.Modules.Catalog.Baskets.Commands.RemoveItem;
using Market.Application.Modules.Catalog.Baskets.Commands.Update;
using Market.Application.Modules.Catalog.Baskets.Commands.UpdateItem;
using Market.Application.Modules.Catalog.Baskets.Queries.GetById;
using Market.Application.Modules.Catalog.Baskets.Queries.GetByUserId;
using Market.Application.Modules.Catalog.Baskets.Queries.List;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class BasketsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateBasketCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateBasketCommand command, CancellationToken ct)
    {
        command.Id = id;
        await sender.Send(command, ct);
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteBasketCommand { Id = id }, ct);
    }

    [HttpGet("by-user/{userId:int}")]
    public async Task<GetBasketByIdQueryDto> GetByUserId(int userId, CancellationToken ct)
    {
        return await sender.Send(new GetBasketByUserIdQuery { UserId = userId }, ct);
    }

    [HttpGet("{id:int}")]
    public async Task<GetBasketByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        return await sender.Send(new GetBasketByIdQuery { Id = id }, ct);
    }

    [HttpGet]
    public async Task<PageResult<ListBasketsQueryDto>> List([FromQuery] ListBasketsQuery query, CancellationToken ct)
    {
        return await sender.Send(query, ct);
    }

    [HttpPost("{basketId:int}/items")]
    public async Task<ActionResult<int>> AddItem(int basketId, AddBasketItemCommand command, CancellationToken ct)
    {
        command.BasketId = basketId;
        int itemId = await sender.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id = basketId }, new { id = itemId });
    }

    [HttpPut("{basketId:int}/items/{itemId:int}")]
    public async Task UpdateItem(int basketId, int itemId, UpdateBasketItemCommand command, CancellationToken ct)
    {
        command.BasketId = basketId;
        command.ItemId = itemId;
        await sender.Send(command, ct);
    }

    [HttpDelete("{basketId:int}/items/{itemId:int}")]
    public async Task RemoveItem(int basketId, int itemId, CancellationToken ct)
    {
        await sender.Send(new RemoveBasketItemCommand { BasketId = basketId, ItemId = itemId }, ct);
    }
}
