namespace Market.Shared.Validation;

public static class PublicIdValidator
{
    public static bool IsValid(string? publicId) =>
        !string.IsNullOrWhiteSpace(publicId) && Guid.TryParse(publicId, out _);

    public static bool IsValidWhenProvided(string? publicId) =>
        string.IsNullOrWhiteSpace(publicId) || Guid.TryParse(publicId, out _);
}
