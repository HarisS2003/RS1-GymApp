using Market.Shared.Constants;

namespace Market.Application.Modules.Catalog.Trainers.Queries.List;

public sealed class ListTrainersQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListTrainersQuery, PageResult<ListTrainersQueryDto>>
{
    public async Task<PageResult<ListTrainersQueryDto>> Handle(ListTrainersQuery request, CancellationToken ct)
    {
        var currentGymId = await GetCurrentGymIdAsync(ct);

        var q =
            from trainer in ctx.Trainers.AsNoTracking()
            join user in ctx.Users.AsNoTracking() on trainer.UserId equals user.Id
            where !trainer.IsDeleted
                  && !user.IsDeleted
                  && user.RoleId == RoleIds.Trainer
                  && trainer.GymId == currentGymId
            select new { trainer, user };

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.trainer.Bio.ToLower().Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(request.UserPublicId))
        {
            var filterUser = await ctx.Users.AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.PublicId == request.UserPublicId && x.GymId == currentGymId,
                    ct)
                ?? throw new MarketNotFoundException("User not found.");

            q = q.Where(x => x.trainer.UserId == filterUser.Id);
        }

        if (request.MinExperienceYears is int minExperienceYears)
            q = q.Where(x => x.trainer.ExperienceYears >= minExperienceYears);

        var projectedQuery = q.Select(x => new ListTrainersQueryDto
        {
            PublicId = x.trainer.PublicId,
            UserPublicId = x.user.PublicId,
            GymId = x.trainer.GymId,
            Bio = x.trainer.Bio,
            ExperienceYears = x.trainer.ExperienceYears
        });

        return await PageResult<ListTrainersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }

    private async Task<int> GetCurrentGymIdAsync(CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var gymId = await ctx.Users.AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => x.GymId)
            .FirstOrDefaultAsync(ct);

        return gymId == 0
            ? throw new MarketNotFoundException("User not found.")
            : gymId;
    }
}
