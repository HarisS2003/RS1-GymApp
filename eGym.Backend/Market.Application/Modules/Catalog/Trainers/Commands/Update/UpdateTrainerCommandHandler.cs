namespace Market.Application.Modules.Catalog.Trainers.Commands.Update;

public sealed class UpdateTrainerCommandHandler(IAppDbContext ctx)
    : IRequestHandler<UpdateTrainerCommand, Unit>
{
    public async Task<Unit> Handle(UpdateTrainerCommand request, CancellationToken ct)
    {
        var entity = await ctx.Trainers.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, ct);
        if (entity is null) throw new MarketNotFoundException($"Trainer (ID={request.Id}) nije pronađen.");

        if (request.UserId <= 0) throw new ValidationException("UserId is required.");
        if (request.GymId <= 0) throw new ValidationException("GymId is required.");

        var normalizedBio = request.Bio.Trim();
        if (string.IsNullOrWhiteSpace(normalizedBio)) throw new ValidationException("Bio is required.");

        if (request.ExperienceYears < 0) throw new ValidationException("ExperienceYears must be zero or positive.");

        if (!await ctx.Users.AnyAsync(x => x.Id == request.UserId, ct))
            throw new ValidationException("Invalid UserId.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid GymId.");

        var exists = await ctx.Trainers.AnyAsync(
            x => x.Id != request.Id
                 && x.UserId == request.UserId
                 && !x.IsDeleted,
            ct);
        if (exists) throw new MarketConflictException("Trainer for this user already exists.");

        entity.UserId = request.UserId;
        entity.GymId = request.GymId;
        entity.Bio = normalizedBio;
        entity.ExperienceYears = request.ExperienceYears;

        await ctx.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
