using System.ComponentModel.DataAnnotations;

namespace eGymSystem.Shared.Options;

public sealed class ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";

    [Required]
    public string Main { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Database=eGymSystemDb;Trusted_Connection=True;TrustServerCertificate=True";
}
