namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.ListMy;

public sealed class ListMyTrainingRequestsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListMyTrainingRequestsQuery, List<ListTrainingRequestQueryDto>>
{
    public async Task<List<ListTrainingRequestQueryDto>> Handle(
        ListMyTrainingRequestsQuery request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId;
        if (userId is null)
            return [];

        return await (
            from r in ctx.TrainingRequests.AsNoTracking()
            join u in ctx.Users.AsNoTracking() on r.UserId equals u.Id
            join t in ctx.Trainers.AsNoTracking() on r.TrainerId equals t.Id
            join tu in ctx.Users.AsNoTracking() on t.UserId equals tu.Id
            where r.UserId == userId
            orderby r.Date descending, r.StartTime descending
            select new ListTrainingRequestQueryDto
            {
                Id = r.Id,
                UserId = r.UserId,
                TrainerId = r.TrainerId,
                MemberName = (u.FirstName + " " + u.LastName).Trim(),
                TrainerName = (tu.FirstName + " " + tu.LastName).Trim(),
                Date = r.Date,
                StartTime = r.StartTime,
                Status = r.Status,
            }
        ).ToListAsync(ct);
    }
}
