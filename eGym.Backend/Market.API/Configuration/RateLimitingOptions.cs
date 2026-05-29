namespace Market.API.Configuration;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    public int PermitLimit { get; set; } = 100;

    public int WindowMinutes { get; set; } = 1;

    public int QueueLimit { get; set; } = 2;
}
