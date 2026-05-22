namespace Market.Application.Modules.Catalog.Products.Commands.Delete;

public class DeleteProductCommandHandler(IAppDbContext ctx)
    : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await ctx.Products
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (product is null)
            throw new MarketNotFoundException("Proizvod nije pronađen.");

        product.IsDeleted = true;
        product.ModifiedAtUtc = DateTime.UtcNow;

        await ctx.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
