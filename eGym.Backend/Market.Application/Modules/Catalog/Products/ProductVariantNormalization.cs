using System.Text.RegularExpressions;

namespace Market.Application.Modules.Catalog.Products;

internal static partial class ProductVariantNormalization
{
    public static string CanonicalSizeKey(string size)
    {
        var raw = size.Trim().ToLowerInvariant().Replace(" ", string.Empty);
        if (string.IsNullOrEmpty(raw))
            return string.Empty;

        var match = SizePattern().Match(raw);
        if (!match.Success)
            return raw;

        var num = match.Groups[1].Value.Replace(',', '.');
        var unit = match.Groups[2].Success ? match.Groups[2].Value.ToLowerInvariant() : "g";
        return $"{num}{unit}";
    }

    public static string CanonicalColorKey(string color)
        => color.Trim().ToLowerInvariant();

    public static string NormalizeSize(string size)
    {
        var trimmed = size.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return trimmed;

        var key = CanonicalSizeKey(trimmed);
        var match = StoredSizePattern().Match(key);
        if (!match.Success)
            return trimmed;

        var num = match.Groups[1].Value;
        var unit = match.Groups[2].Success ? match.Groups[2].Value : "g";
        return $"{num}{unit}";
    }

    [GeneratedRegex(@"^(\d+(?:[.,]\d+)?)(kg|g|ml|l|xl|xxl|xs|s|m)?$", RegexOptions.IgnoreCase)]
    private static partial Regex SizePattern();

    [GeneratedRegex(@"^(\d+(?:\.\d+)?)(kg|g|ml|l|xl|xxl|xs|s|m)?$", RegexOptions.IgnoreCase)]
    private static partial Regex StoredSizePattern();
}
