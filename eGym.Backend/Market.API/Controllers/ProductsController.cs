using Market.Application.Modules.Catalog.Products.Queries.List;
using Market.Application.Modules.Catalog.Products.Queries.GetById;
using Market.Application.Modules.Catalog.Products.Commands.Create;
using Market.Application.Modules.Catalog.Products.Commands.Delete;
using Market.Application.Modules.Catalog.Products.Commands.Update;
using Market.API.Configuration;
using Microsoft.AspNetCore.RateLimiting;

namespace Market.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting(RateLimitingServiceExtensions.ApiFixedWindowPolicy)]
public class ProductsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProductCommand command, CancellationToken ct)
    {
        int id = await sender.Send(command, ct);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task Update(int id, UpdateProductCommand command, CancellationToken ct)
    {
        // ID from the route takes precedence
        command.Id = id;
        await sender.Send(command, ct);
        // no return -> 204 No Content
    }

    [HttpDelete("{id:int}")]
    public async Task Delete(int id, CancellationToken ct)
    {
        await sender.Send(new DeleteProductCommand { Id = id }, ct);
        // no return -> 204 No Content
    }

    /// <summary>Returns product with all variants (size / color-or-flavor) in ProductVariants.</summary>
    [HttpGet("{id:int}")]
    public async Task<GetProductByIdQueryDto> GetById(int id, CancellationToken ct)
    {
        var category = await sender.Send(new GetProductByIdQuery { Id = id }, ct);
        return category; // if NotFoundException -> 404 via middleware
    }

    /// <summary>Optional query: size (e.g. 1kg, XL) filters products that have a matching variant.</summary>
    [HttpGet]
    public async Task<PageResult<ListProductsQueryDto>> List([FromQuery] ListProductsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result;
    }

    //[HttpPut("{id:int}/disable")]
    //public async Task Disable(int id, CancellationToken ct)
    //{
    //    await sender.Send(new DisableProductCategoryCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}

    //[HttpPut("{id:int}/enable")]
    //public async Task Enable(int id, CancellationToken ct)
    //{
    //    await sender.Send(new EnableProductCategoryCommand { Id = id }, ct);
    //    // no return -> 204 No Content
    //}
}