namespace Market.Application.Modules.Catalog.Trainers.Queries.List;

public sealed class ListTrainersQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListTrainersQuery, PageResult<ListTrainersQueryDto>>
{
    public async Task<PageResult<ListTrainersQueryDto>> Handle(ListTrainersQuery request, CancellationToken ct)
    {
        var q = ctx.Trainers.AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x => x.Bio.ToLower().Contains(searchTerm));
        }

        if (request.GymId is int gymId)
            q = q.Where(x => x.GymId == gymId);

        if (request.UserId is int userId)
            q = q.Where(x => x.UserId == userId);

        if (request.MinExperienceYears is int minExperienceYears)
            q = q.Where(x => x.ExperienceYears >= minExperienceYears);

        var projectedQuery = q.Select(x => new ListTrainersQueryDto
        {
            Id = x.Id,
            UserId = x.UserId,
            GymId = x.GymId,
            Bio = x.Bio,
            ExperienceYears = x.ExperienceYears
        });

        return await PageResult<ListTrainersQueryDto>.FromQueryableAsync(projectedQuery, request.Paging, ct);
    }
}
