using System.Text.RegularExpressions;

namespace Market.Shared.Validation;

public static partial class BosnianPhoneNumberValidator
{
    private static readonly HashSet<string> LocalMobilePrefixes =
    [
        "060", "061", "062", "063", "064", "065", "066", "067", "068", "069"
    ];

    private static readonly HashSet<string> LocalLandlinePrefixes = ["033", "034", "035", "036", "037", "038", "039"];

    public static bool IsValid(string? input, out string? normalized)
    {
        normalized = null;
        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var compact = Compact(input);
        if (compact.Length == 0)
        {
            return false;
        }

        if (TryNormalizeInternational(compact, out normalized))
        {
            return true;
        }

        if (TryNormalizeLocal(compact, out normalized))
        {
            return true;
        }

        return false;
    }

    public static string NormalizeOrThrow(string input)
    {
        if (!IsValid(input, out var normalized) || string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Invalid Bosnian phone number format.", nameof(input));
        }

        return normalized;
    }

    private static string Compact(string input)
    {
        var trimmed = input.Trim();
        return Regex.Replace(trimmed, @"[\s\-()]", string.Empty);
    }

    private static bool TryNormalizeInternational(string compact, out string? normalized)
    {
        normalized = null;
        var digits = compact.StartsWith('+') ? compact[1..] : compact;

        if (!digits.StartsWith("387", StringComparison.Ordinal))
        {
            return false;
        }

        var rest = digits[3..];
        if (rest.Length != 8 && rest.Length != 9)
        {
            return false;
        }

        if (!rest.All(char.IsDigit))
        {
            return false;
        }

        if (!IsValidInternationalRest(rest))
        {
            return false;
        }

        normalized = $"+387{rest}";
        return true;
    }

    private static bool TryNormalizeLocal(string compact, out string? normalized)
    {
        normalized = null;

        if (!compact.StartsWith('0') || compact.Length != 9)
        {
            return false;
        }

        if (!compact.All(char.IsDigit))
        {
            return false;
        }

        var prefix = compact[..3];
        if (!LocalMobilePrefixes.Contains(prefix) && !LocalLandlinePrefixes.Contains(prefix))
        {
            return false;
        }

        normalized = $"+387{compact[1..]}";
        return true;
    }

    private static bool IsValidInternationalRest(string rest)
    {
        if (rest.Length == 8)
        {
            return rest[0] == '6' || rest.StartsWith("33", StringComparison.Ordinal);
        }

        if (rest.Length == 9 && rest.StartsWith("33", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }
}
