using Market.Application.Modules.Identity.UserMemberships;
using Market.Application.Modules.Identity.UserMemberships.Services;
using Market.Shared.Validation;
using Microsoft.EntityFrameworkCore;

namespace Market.Infrastructure.Database.Seeders;

public static class DynamicDataSeeder
{
    public static async Task SeedAsync(DatabaseContext context)
    {
        var users = await context.Users
            .OrderBy(u => u.Id)
            .ToListAsync();

        if (users.Count == 0)
        {
            return;
        }

        var changed = false;
        for (var i = 0; i < users.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(users[i].PhoneNumber)
                && BosnianPhoneNumberValidator.IsValid(users[i].PhoneNumber, out _))
            {
                continue;
            }

            // Unique Bosnian mobiles: +387 6X XXXXXX (8 digits after country code)
            var operatorCode = 60 + (i % 10);
            var subscriber = 100_000 + i;
            users[i].PhoneNumber = $"+387{operatorCode}{subscriber:D6}";
            changed = true;
        }

        if (changed)
        {
            await context.SaveChangesAsync();
        }

        await BackfillMembershipCreatedEventsAsync(context);
    }

    private static async Task BackfillMembershipCreatedEventsAsync(DatabaseContext context)
    {
        var memberships = await context.UserMemberships
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new
            {
                x.Id,
                x.UserId,
                x.MembershipPlanId,
                x.StartDate,
                x.EndDate,
                x.CreatedAtUtc,
            })
            .ToListAsync();

        if (memberships.Count == 0)
        {
            return;
        }

        var membershipIdsWithCreatedEvent = await context.MembershipEvents
            .AsNoTracking()
            .Where(x => x.EventType == MembershipEventTypes.Created || x.EventType == "MembershipPurchased")
            .Where(x => x.UserMembershipId.HasValue)
            .Select(x => x.UserMembershipId!.Value)
            .Distinct()
            .ToListAsync();

        var missing = memberships
            .Where(x => !membershipIdsWithCreatedEvent.Contains(x.Id))
            .ToList();

        if (missing.Count == 0)
        {
            return;
        }

        var planNames = await context.MembershipPlans
            .AsNoTracking()
            .Where(x => missing.Select(m => m.MembershipPlanId).Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, x => x.Name);

        foreach (var membership in missing)
        {
            planNames.TryGetValue(membership.MembershipPlanId, out var planName);

            MembershipEventRecorder.Append(
                context,
                membership.Id,
                MembershipEventTypes.Created,
                new
                {
                    userId = membership.UserId,
                    membershipPlanId = membership.MembershipPlanId,
                    planName = planName ?? "Membership",
                    startDate = membership.StartDate,
                    endDate = membership.EndDate,
                    backfilled = true,
                },
                membership.CreatedAtUtc == default ? membership.StartDate : membership.CreatedAtUtc);
        }

        await context.SaveChangesAsync();
    }
}
