using Market.Application.Modules.Identity.UserMemberships.Services;

namespace Market.Application.Modules.Catalog.Users.Queries.ListWithMembership;

public sealed class ListUsersWithMembershipQueryHandler(IAppDbContext ctx)
    : IRequestHandler<ListUsersWithMembershipQuery, PageResult<ListUsersWithMembershipQueryDto>>
{
    public async Task<PageResult<ListUsersWithMembershipQueryDto>> Handle(
        ListUsersWithMembershipQuery request,
        CancellationToken ct)
    {
        var today = DateTime.UtcNow.Date;
        var q = ctx.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim().ToLower();
            q = q.Where(x =>
                x.FirstName.ToLower().Contains(searchTerm)
                || x.LastName.ToLower().Contains(searchTerm)
                || x.Email.ToLower().Contains(searchTerm));
        }

        if (request.GymId is int gymId)
        {
            q = q.Where(x => x.GymId == gymId);
        }

        if (request.RoleId is int roleId)
        {
            q = q.Where(x => x.RoleId == roleId);
        }

        var page = await PageResult<ListUsersWithMembershipQueryDto>.FromQueryableAsync(
            q.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Select(x => new ListUsersWithMembershipQueryDto
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                UserMembershipId = null,
                CurrentMembershipName = null,
                MembershipStatus = "None",
            }),
            request.Paging,
            ct);

        var userIds = page.Items.Select(x => x.Id).ToList();
        if (userIds.Count == 0)
        {
            return page;
        }

        var activeMemberships = await (
            from m in ctx.UserMemberships.AsNoTracking()
            join p in ctx.MembershipPlans.AsNoTracking() on m.MembershipPlanId equals p.Id into plans
            from p in plans.DefaultIfEmpty()
            where userIds.Contains(m.UserId) && m.EndDate.Date >= today
            orderby m.EndDate descending
            select new
            {
                m.Id,
                m.UserId,
                PlanName = p != null ? p.Name : "Membership",
            }
        ).ToListAsync(ct);

        var membershipByUser = activeMemberships
            .GroupBy(x => x.UserId)
            .ToDictionary(g => g.Key, g => g.First());

        var membershipIds = membershipByUser.Values.Select(x => x.Id).ToList();
        var events = membershipIds.Count == 0
            ? []
            : await ctx.MembershipEvents.AsNoTracking()
                .Where(x => x.UserMembershipId.HasValue && membershipIds.Contains(x.UserMembershipId.Value))
                .ToListAsync(ct);

        var eventsByMembership = events
            .Where(x => x.UserMembershipId.HasValue)
            .GroupBy(x => x.UserMembershipId!.Value)
            .ToDictionary(g => g.Key, g => g.AsEnumerable());

        var now = DateTime.UtcNow;
        var items = page.Items.Select(user =>
        {
            if (!membershipByUser.TryGetValue(user.Id, out var membership))
            {
                return user;
            }

            var status = "Active";
            if (eventsByMembership.TryGetValue(membership.Id, out var membershipEvents))
            {
                var replayed = MembershipEventReplayer.Replay(membershipEvents, now);
                status = replayed.Status;
            }

            return new ListUsersWithMembershipQueryDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserMembershipId = membership.Id,
                CurrentMembershipName = membership.PlanName,
                MembershipStatus = status,
            };
        }).ToList();

        return new PageResult<ListUsersWithMembershipQueryDto>
        {
            Items = items,
            TotalItems = page.TotalItems,
            CurrentPage = page.CurrentPage,
            PageSize = page.PageSize,
            IncludedTotal = page.IncludedTotal,
            TotalPages = page.TotalPages,
        };
    }
}
