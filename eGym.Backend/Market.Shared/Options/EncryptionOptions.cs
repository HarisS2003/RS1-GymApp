using System.ComponentModel.DataAnnotations;

namespace Market.Shared.Options;

public sealed class EncryptionOptions
{
    public const string SectionName = "Encryption";

    /// <summary>Base64-encoded 32-byte key for AES-256.</summary>
    [Required]
    [MinLength(44)]
    public string Key { get; set; } = string.Empty;
}
