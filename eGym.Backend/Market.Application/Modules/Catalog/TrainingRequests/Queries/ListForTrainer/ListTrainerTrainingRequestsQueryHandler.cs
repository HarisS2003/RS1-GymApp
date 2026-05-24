namespace Market.Application.Modules.Catalog.TrainingRequests.Queries.ListForTrainer;

public sealed class ListTrainerTrainingRequestsQueryHandler(IAppDbContext ctx, IAppCurrentUser currentUser)
    : IRequestHandler<ListTrainerTrainingRequestsQuery, List<ListTrainerTrainingRequestQueryDto>>
{
    public async Task<List<ListTrainerTrainingRequestQueryDto>> Handle(
        ListTrainerTrainingRequestsQuery request,
        CancellationToken ct)
    {
        var userId = currentUser.UserId
            ?? throw new ValidationException("Current user is required.");

        var trainer = await ctx.Trainers.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, ct)
            ?? throw new ValidationException("Trainer profile not found for current user.");

        var q = ctx.TrainingRequests.AsNoTracking()
            .Where(x => x.TrainerId == trainer.Id);

        if (request.Status is TrainingRequestStatus status)
            q = q.Where(x => x.Status == status);

        var rows = await (
            from r in q
            join u in ctx.Users.AsNoTracking() on r.UserId equals u.Id
            orderby r.Date, r.StartTime
            select new ListTrainerTrainingRequestQueryDto
            {
                Id = r.Id,
                UserId = r.UserId,
                TrainerId = r.TrainerId,
                MemberName = (u.FirstName + " " + u.LastName).Trim(),
                Date = r.Date,
                StartTime = r.StartTime,
                Status = r.Status,
            }
        ).ToListAsync(ct);

        return rows
            .OrderBy(x => x.Status == TrainingRequestStatus.Pending ? 0 : 1)
            .ThenBy(x => x.Date)
            .ThenBy(x => x.StartTime)
            .ToList();
    }
}
