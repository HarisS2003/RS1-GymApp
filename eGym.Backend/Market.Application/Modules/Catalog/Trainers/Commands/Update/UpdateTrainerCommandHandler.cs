namespace Market.Application.Modules.Catalog.Trainers.Commands.Update;

public sealed class UpdateTrainerCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateTrainerCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTrainerCommand request, CancellationToken ct)
    {
        var entity = await ctx.Trainers.FirstOrDefaultAsync(
            x => x.PublicId == request.PublicId && x.GymId == request.GymId && !x.IsDeleted,
            ct);
        if (entity is null) throw new MarketNotFoundException("Trainer not found.");

        var user = await ctx.Users.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.PublicId == request.UserPublicId && x.GymId == request.GymId && !x.IsDeleted,
                ct)
            ?? throw new ValidationException("Invalid UserId.");

        var userId = user.Id;

        var normalizedBio = request.Bio.Trim();
        if (string.IsNullOrWhiteSpace(normalizedBio)) throw new ValidationException("Bio is required.");

        if (request.ExperienceYears < 0) throw new ValidationException("ExperienceYears must be zero or positive.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid GymId.");

        var exists = await ctx.Trainers.AnyAsync(
            x => x.Id != entity.Id
                 && x.UserId == userId
                 && !x.IsDeleted,
            ct);
        if (exists) throw new MarketConflictException("Trainer for this user already exists.");

        entity.UserId = userId;
        entity.GymId = request.GymId;
        entity.Bio = normalizedBio;
        entity.ExperienceYears = request.ExperienceYears;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
