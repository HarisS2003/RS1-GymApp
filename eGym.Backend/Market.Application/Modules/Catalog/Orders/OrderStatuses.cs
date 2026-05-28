namespace Market.Application.Modules.Catalog.Orders;

public static class OrderStatuses
{
    public const string Draft = "Draft";
    public const string Confirmed = "Confirmed";
    public const string Cancelled = "Cancelled";

    private static readonly HashSet<string> All =
        new(StringComparer.OrdinalIgnoreCase) { Draft, Confirmed, Cancelled };

    public static bool IsValid(string status) => All.Contains(status);
}
