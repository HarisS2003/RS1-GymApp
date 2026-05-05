namespace Market.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var normalized = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ValidationException("Name is required.");

        var gymExists = await ctx.Gyms.AnyAsync(x => x.Id == request.GymId, ct);
        if (!gymExists) throw new ValidationException("Invalid GymId.");

        var exists = await ctx.Products.AnyAsync(x => x.Name == normalized && x.GymId == request.GymId, ct);
        if (exists) throw new MarketConflictException("Name already exists in gym.");

        var product = new ProductEntity
        {
            Name = normalized,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            GymId = request.GymId
        };

        ctx.Products.Add(product);
        await ctx.SaveChangesAsync(ct);
        return product.Id;
    }
}
