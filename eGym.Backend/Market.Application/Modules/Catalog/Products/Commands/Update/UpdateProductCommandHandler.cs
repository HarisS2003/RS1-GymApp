namespace Market.Application.Modules.Catalog.Products.Commands.Update;

public sealed class UpdateProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var entity = await ctx.Products.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null) throw new MarketNotFoundException($"Product (ID={request.Id}) nije pronađen.");

        var gymExists = await ctx.Gyms.AnyAsync(x => x.Id == request.GymId, ct);
        if (!gymExists) throw new ValidationException("Invalid GymId.");

        var exists = await ctx.Products.AnyAsync(x => x.Id != request.Id && x.Name.ToLower() == request.Name.ToLower() && x.GymId == request.GymId, ct);
        if (exists) throw new MarketConflictException("Name already exists in gym.");

        entity.Name = request.Name.Trim();
        entity.Price = request.Price;
        entity.StockQuantity = request.StockQuantity;
        entity.GymId = request.GymId;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
