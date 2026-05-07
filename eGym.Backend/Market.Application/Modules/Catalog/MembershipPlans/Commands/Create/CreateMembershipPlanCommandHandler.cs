namespace Market.Application.Modules.Catalog.MembershipPlans.Commands.Create;

public class CreateMembershipPlanCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateMembershipPlanCommand, int>
{
    public async Task<int> Handle(CreateMembershipPlanCommand request, CancellationToken ct)
    {
        var normalized = request.Name.Trim();
        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException("Name is required.");

        if (request.DurationDays <= 0)
            throw new ValidationException("DurationDays must be greater than zero.");

        if (request.DiscountPercentage < 0 || request.DiscountPercentage > 100)
            throw new ValidationException("DiscountPercentage must be between 0 and 100.");

        var gymExists = await ctx.Gyms.AnyAsync(x => x.Id == request.GymId, ct);
        if (!gymExists)
            throw new ValidationException("Invalid GymId.");

        var exists = await ctx.MembershipPlans.AnyAsync(
            x => x.Name.ToLower() == normalized.ToLower() && x.GymId == request.GymId,
            ct);
        if (exists)
            throw new MarketConflictException("Name already exists in gym.");

        var entity = new MembershipPlanEntity
        {
            Name = normalized,
            DurationDays = request.DurationDays,
            Price = request.Price,
            DiscountPercentage = request.DiscountPercentage,
            GymId = request.GymId
        };

        ctx.MembershipPlans.Add(entity);
        await ctx.SaveChangesAsync(ct);
        return entity.Id;
    }
}
