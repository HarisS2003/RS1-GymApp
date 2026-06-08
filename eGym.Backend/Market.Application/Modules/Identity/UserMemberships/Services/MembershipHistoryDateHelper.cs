namespace Market.Application.Modules.Identity.UserMemberships.Services;

public static class MembershipHistoryDateHelper
{
    public static DateTime NormalizeAsOfDate(DateTime value)
    {
        if (value == default)
        {
            return DateTime.UtcNow;
        }

        var utc = ToUtc(value);
        return utc.Date.AddDays(1).AddTicks(-1);
    }

    public static DateTime AsOfCalendarDate(DateTime value)
    {
        if (value == default)
        {
            return DateTime.UtcNow.Date;
        }

        return ToUtc(value).Date;
    }

    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc),
    };
}
