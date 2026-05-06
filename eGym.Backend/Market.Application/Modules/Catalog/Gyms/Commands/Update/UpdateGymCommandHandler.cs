namespace Market.Application.Modules.Catalog.Gyms.Commands.Update;

public sealed class UpdateGymCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateGymCommand, Unit>
{
    public async Task<Unit> Handle(UpdateGymCommand request, CancellationToken ct)
    {
        var entity = await ctx.Gyms.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
        if (entity is null) throw new MarketNotFoundException($"Gym (ID={request.Id}) nije pronađen.");

        var normalizedName = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName)) throw new ValidationException("Name is required.");

        var normalizedAddress = request.Address.Trim();
        if (string.IsNullOrWhiteSpace(normalizedAddress)) throw new ValidationException("Address is required.");

        var normalizedCity = request.City.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCity)) throw new ValidationException("City is required.");

        var exists = await ctx.Gyms.AnyAsync(
            x => x.Id != request.Id
                 && !x.IsDeleted
                 && x.Name.ToLower() == normalizedName.ToLower()
                 && x.City.ToLower() == normalizedCity.ToLower(),
            ct);
        if (exists) throw new MarketConflictException("Gym already exists in city.");

        entity.Name = normalizedName;
        entity.Address = normalizedAddress;
        entity.City = normalizedCity;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
