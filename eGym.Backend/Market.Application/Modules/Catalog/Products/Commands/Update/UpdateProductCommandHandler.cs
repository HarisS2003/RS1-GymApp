namespace Market.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var entity = await ctx.Products.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) throw new MarketNotFoundException($"Product (ID={request.Id}) nije pronađen.");

        var normalizedName = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName)) throw new ValidationException("Name is required.");

        var categoryName = request.CategoryName.Trim();
        if (string.IsNullOrWhiteSpace(categoryName)) throw new ValidationException("CategoryName is required.");

        if (request.Price < 0) throw new ValidationException("Price must be zero or positive.");
        if (request.StockQuantity < 0) throw new ValidationException("StockQuantity must be zero or positive.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid GymId.");

        var exists = await ctx.Products.AnyAsync(
            x => x.Id != request.Id && x.Name.ToLower() == normalizedName.ToLower() && x.GymId == request.GymId, ct);
        if (exists) throw new MarketConflictException("Name already exists in gym.");

        entity.Name = normalizedName;
        entity.CategoryName = categoryName;
        entity.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();
        entity.Price = request.Price;
        entity.StockQuantity = request.StockQuantity;
        entity.GymId = request.GymId;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
