using System.ComponentModel.DataAnnotations;

namespace eGymSystem.Shared.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; set; } = "eGymSystem";

    [Required]
    public string Audience { get; set; } = "eGymSystem.Client";

    [Required]
    [MinLength(16)]
    public string Key { get; set; } = "change-this-dev-key-123";
}
