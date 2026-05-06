namespace Market.Application.Modules.Catalog.Gyms.Commands.Create;

public class CreateGymCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateGymCommand, int>
{
    public async Task<int> Handle(CreateGymCommand request, CancellationToken ct)
    {
        var normalizedName = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName)) throw new ValidationException("Name is required.");

        var normalizedAddress = request.Address.Trim();
        if (string.IsNullOrWhiteSpace(normalizedAddress)) throw new ValidationException("Address is required.");

        var normalizedCity = request.City.Trim();
        if (string.IsNullOrWhiteSpace(normalizedCity)) throw new ValidationException("City is required.");

        var exists = await ctx.Gyms.AnyAsync(
            x => !x.IsDeleted
                 && x.Name.ToLower() == normalizedName.ToLower()
                 && x.City.ToLower() == normalizedCity.ToLower(),
            ct);
        if (exists) throw new MarketConflictException("Gym already exists in city.");

        var gym = new GymEntity
        {
            Name = normalizedName,
            Address = normalizedAddress,
            City = normalizedCity
        };

        ctx.Gyms.Add(gym);
        await ctx.SaveChangesAsync(ct);
        return gym.Id;
    }
}
