using System.Text.RegularExpressions;

namespace CommonUtils.Extensions;

/// <summary>
/// Extension methods for the string operations that appear in nearly every API:
/// normalizing user input, capping lengths, and converting naming conventions.
/// </summary>
public static class StringExtensions
{
    private static readonly Regex WhitespacePattern =
        new(@"\s+", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private static readonly Regex PascalOrCamelPattern =
        new(@"([a-z0-9])([A-Z])", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    private static readonly Regex SeparatorPattern =
        new(@"[-_](\w)", RegexOptions.Compiled, TimeSpan.FromSeconds(1));

    /// <summary>
    /// Trims leading/trailing whitespace and collapses any run of internal whitespace
    /// to a single space. Useful for cleaning up name/address fields from form input.
    /// </summary>
    public static string Normalize(this string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return WhitespacePattern.Replace(value.Trim(), " ");
    }

    /// <summary>
    /// Returns the string as-is if it fits within <paramref name="maxLength"/>,
    /// or cuts it to exactly <paramref name="maxLength"/> characters without throwing.
    /// </summary>
    public static string Truncate(this string value, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLength);
        return value.Length <= maxLength ? value : value[..maxLength];
    }

    /// <summary>
    /// Returns <c>null</c> if the string is null, empty, or whitespace-only;
    /// otherwise returns the original value. Handy for optional fields that should
    /// be stored as NULL in the database rather than as empty strings.
    /// </summary>
    public static string? NullIfEmpty(this string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>
    /// Converts PascalCase or camelCase to snake_case.
    /// Useful when mapping C# property names to JSON keys, query params, or DB columns
    /// without a serializer attribute on every property.
    /// <example><c>"OrderId"</c> → <c>"order_id"</c></example>
    /// </summary>
#pragma warning disable CA1308 // Intentionally lowercasing for snake_case / kebab-case output
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return PascalOrCamelPattern.Replace(value, "$1_$2").ToLowerInvariant();
    }

    /// <summary>
    /// Converts PascalCase or camelCase to kebab-case.
    /// Useful for URL slugs and route segments.
    /// <example><c>"OrderLineItem"</c> → <c>"order-line-item"</c></example>
    /// </summary>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return PascalOrCamelPattern.Replace(value, "$1-$2").ToLowerInvariant();
    }
#pragma warning restore CA1308

    /// <summary>
    /// Converts snake_case or kebab-case to PascalCase.
    /// <example><c>"order_line_item"</c> → <c>"OrderLineItem"</c></example>
    /// </summary>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var seeded = char.ToUpperInvariant(value[0]) + (value.Length > 1 ? value[1..] : string.Empty);
        return SeparatorPattern.Replace(seeded, m => m.Groups[1].Value.ToUpperInvariant());
    }

    /// <summary>
    /// Masks the middle of a string, preserving <paramref name="visibleStart"/> characters at the
    /// front and <paramref name="visibleEnd"/> characters at the end. Safe for logging emails,
    /// tokens, and phone numbers.
    /// <example><c>"user@example.com".Mask(2, 4)</c> → <c>"us**********m"</c></example>
    /// </summary>
    public static string Mask(this string value, int visibleStart = 2, int visibleEnd = 0, char maskChar = '*')
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleStart);
        ArgumentOutOfRangeException.ThrowIfNegative(visibleEnd);

        var totalVisible = visibleStart + visibleEnd;
        if (totalVisible >= value.Length)
            return new string(maskChar, value.Length);

        var start = visibleStart > 0 ? value[..visibleStart] : string.Empty;
        var end = visibleEnd > 0 ? value[^visibleEnd..] : string.Empty;
        return start + new string(maskChar, value.Length - totalVisible) + end;
    }
}
