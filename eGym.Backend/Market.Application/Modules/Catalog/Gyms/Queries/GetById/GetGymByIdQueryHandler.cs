namespace Market.Application.Modules.Catalog.Gyms.Queries.GetById;

public sealed class GetGymByIdQueryHandler(IAppDbContext ctx) : IRequestHandler<GetGymByIdQuery, GetGymByIdQueryDto>
{
    public async Task<GetGymByIdQueryDto> Handle(GetGymByIdQuery request, CancellationToken ct)
    {
        var dto = await ctx.Gyms.AsNoTracking()
            .Where(x => x.Id == request.Id && !x.IsDeleted)
            .Select(x => new GetGymByIdQueryDto
            {
                Id = x.Id,
                Name = x.Name,
                Address = x.Address,
                City = x.City
            })
            .FirstOrDefaultAsync(ct);

        return dto ?? throw new MarketNotFoundException($"Gym (ID={request.Id}) nije pronađen.");
    }
}
