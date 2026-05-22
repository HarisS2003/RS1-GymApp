namespace Market.Application.Modules.Catalog.Products.Commands.Create;

public class CreateProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var normalized = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalized)) throw new ValidationException("Name is required.");

        var categoryName = request.CategoryName.Trim();
        if (string.IsNullOrWhiteSpace(categoryName)) throw new ValidationException("CategoryName is required.");

        if (request.Price < 0) throw new ValidationException("Price must be zero or positive.");
        if (request.StockQuantity < 0) throw new ValidationException("StockQuantity must be zero or positive.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid GymId.");

        var activeExists = await ctx.Products.AnyAsync(
            x => x.Name == normalized && x.GymId == request.GymId, ct);
        if (activeExists) throw new MarketConflictException("Name already exists in gym.");

        var deleted = await ctx.Products
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Name == normalized && x.GymId == request.GymId && x.IsDeleted, ct);

        if (deleted is not null)
        {
            deleted.IsDeleted = false;
            deleted.CategoryName = categoryName;
            deleted.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
            deleted.Price = request.Price;
            deleted.StockQuantity = request.StockQuantity;
            deleted.ModifiedAtUtc = DateTime.UtcNow;

            await ctx.SaveChangesAsync(ct);
            return deleted.Id;
        }

        var product = new ProductEntity
        {
            Name = normalized,
            CategoryName = categoryName,
            Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            GymId = request.GymId
        };

        ctx.Products.Add(product);
        await ctx.SaveChangesAsync(ct);
        return product.Id;
    }
}
