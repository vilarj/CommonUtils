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
#pragma warning disable CA1308 // Intentionally lowercasing for snake_case output
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return PascalOrCamelPattern.Replace(value, "$1_$2").ToLowerInvariant();
    }
#pragma warning restore CA1308
}
