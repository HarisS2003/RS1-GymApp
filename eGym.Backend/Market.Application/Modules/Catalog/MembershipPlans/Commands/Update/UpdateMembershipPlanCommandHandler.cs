namespace Market.Application.Modules.Catalog.MembershipPlans.Commands.Update;

public sealed class UpdateMembershipPlanCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateMembershipPlanCommand, Unit>
{
    public async Task<Unit> Handle(UpdateMembershipPlanCommand request, CancellationToken ct)
    {
        var entity = await ctx.MembershipPlans.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
        if (entity is null)
            throw new MarketNotFoundException($"Membership plan (ID={request.Id}) nije pronađen.");

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
            x => x.Id != request.Id
                 && x.Name.ToLower() == normalized.ToLower()
                 && x.GymId == request.GymId,
            ct);
        if (exists)
            throw new MarketConflictException("Name already exists in gym.");

        entity.Name = normalized;
        entity.DurationDays = request.DurationDays;
        entity.Price = request.Price;
        entity.DiscountPercentage = request.DiscountPercentage;
        entity.GymId = request.GymId;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
