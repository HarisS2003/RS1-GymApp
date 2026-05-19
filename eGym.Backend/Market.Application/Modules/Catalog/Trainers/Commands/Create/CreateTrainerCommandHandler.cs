namespace Market.Application.Modules.Catalog.Trainers.Commands.Create;

public sealed class CreateTrainerCommandHandler(IAppDbContext ctx)
    : IRequestHandler<CreateTrainerCommand, int>
{
    public async Task<int> Handle(CreateTrainerCommand request, CancellationToken ct)
    {
        if (request.UserId <= 0) throw new ValidationException("UserId is required.");
        if (request.GymId <= 0) throw new ValidationException("GymId is required.");

        var normalizedBio = request.Bio.Trim();
        if (string.IsNullOrWhiteSpace(normalizedBio)) throw new ValidationException("Bio is required.");

        if (request.ExperienceYears < 0) throw new ValidationException("ExperienceYears must be zero or positive.");

        if (!await ctx.Users.AnyAsync(x => x.Id == request.UserId, ct))
            throw new ValidationException("Invalid UserId.");

        if (!await ctx.Gyms.AnyAsync(x => x.Id == request.GymId && !x.IsDeleted, ct))
            throw new ValidationException("Invalid GymId.");

        var exists = await ctx.Trainers.AnyAsync(x => x.UserId == request.UserId && !x.IsDeleted, ct);
        if (exists) throw new MarketConflictException("Trainer for this user already exists.");

        var trainer = new TrainerEntity
        {
            UserId = request.UserId,
            GymId = request.GymId,
            Bio = normalizedBio,
            ExperienceYears = request.ExperienceYears
        };

        ctx.Trainers.Add(trainer);
        await ctx.SaveChangesAsync(ct);
        return trainer.Id;
    }
}
