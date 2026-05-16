using System.Text.RegularExpressions;

namespace CommonUtils.Checks;

/// <summary>
/// Static guard class. Every method validates a value and returns it unchanged
/// if valid, so checks can be composed inline or at the top of a method body.
/// Throws standard .NET argument exceptions; callers (middleware, filters) decide
/// how to translate them into HTTP responses.
/// </summary>
public static class Check
{
    private static readonly Regex EmailPattern =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));

    // ── Null ─────────────────────────────────────────────────────────────────

    /// <summary>Throws <see cref="ArgumentNullException"/> if <paramref name="value"/> is null.</summary>
    public static T NotNull<T>(T? value, string paramName) where T : class
    {
        if (value is null)
            throw new ArgumentNullException(paramName);

        return value;
    }

    // ── String ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Throws if <paramref name="value"/> is null or whitespace.
    /// Returns the trimmed value on success.
    /// </summary>
    public static string NotEmpty(string? value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value must not be null or whitespace.", paramName);

        return value!.Trim();
    }

    /// <summary>Throws if the string (after trimming) exceeds <paramref name="max"/> characters.</summary>
    public static string MaxLength(string value, int max, string paramName)
    {
        NotEmpty(value, paramName);
        if (value.Trim().Length > max)
            throw new ArgumentException($"Value must not exceed {max} characters.", paramName);

        return value;
    }

    /// <summary>Throws if the string (after trimming) is shorter than <paramref name="min"/> characters.</summary>
    public static string MinLength(string value, int min, string paramName)
    {
        NotEmpty(value, paramName);
        if (value.Trim().Length < min)
            throw new ArgumentException($"Value must be at least {min} characters.", paramName);

        return value;
    }

    /// <summary>Throws if the trimmed string length is outside [<paramref name="min"/>, <paramref name="max"/>].</summary>
    public static string Length(string value, int min, int max, string paramName)
    {
        MinLength(value, min, paramName);
        MaxLength(value, max, paramName);
        return value;
    }

    // ── Integer ──────────────────────────────────────────────────────────────

    /// <summary>Throws if <paramref name="value"/> is zero or negative.</summary>
    public static int Positive(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero.");

        return value;
    }

    /// <summary>Throws if <paramref name="value"/> is negative.</summary>
    public static int NotNegative(int value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be zero or greater.");

        return value;
    }

    /// <summary>Throws if <paramref name="value"/> falls outside [<paramref name="min"/>, <paramref name="max"/>].</summary>
    public static int InRange(int value, int min, int max, string paramName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");

        return value;
    }

    // ── Long ─────────────────────────────────────────────────────────────────

    /// <inheritdoc cref="Positive(int, string)"/>
    public static long Positive(long value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero.");

        return value;
    }

    /// <inheritdoc cref="NotNegative(int, string)"/>
    public static long NotNegative(long value, string paramName)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be zero or greater.");

        return value;
    }

    // ── Decimal ──────────────────────────────────────────────────────────────

    /// <inheritdoc cref="Positive(int, string)"/>
    public static decimal Positive(decimal value, string paramName)
    {
        if (value <= 0m)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero.");

        return value;
    }

    /// <inheritdoc cref="NotNegative(int, string)"/>
    public static decimal NotNegative(decimal value, string paramName)
    {
        if (value < 0m)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be zero or greater.");

        return value;
    }

    /// <inheritdoc cref="InRange(int, int, int, string)"/>
    public static decimal InRange(decimal value, decimal min, decimal max, string paramName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(paramName, value, $"Value must be between {min} and {max}.");

        return value;
    }

    // ── Double ───────────────────────────────────────────────────────────────

    /// <inheritdoc cref="Positive(int, string)"/>
    public static double Positive(double value, string paramName)
    {
        if (value <= 0d)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be greater than zero.");

        return value;
    }

    /// <inheritdoc cref="NotNegative(int, string)"/>
    public static double NotNegative(double value, string paramName)
    {
        if (value < 0d)
            throw new ArgumentOutOfRangeException(paramName, value, "Value must be zero or greater.");

        return value;
    }

    // ── Collections ──────────────────────────────────────────────────────────

    /// <summary>Throws if the collection is null or contains no elements.</summary>
    public static IEnumerable<T> NotEmpty<T>(IEnumerable<T>? value, string paramName)
    {
        var result = NotNull(value, paramName);
        if (!result.Any())
            throw new ArgumentException("Collection must not be empty.", paramName);

        return result;
    }

    /// <summary>Throws if the collection exceeds <paramref name="max"/> items.</summary>
    public static ICollection<T> MaxCount<T>(ICollection<T> value, int max, string paramName)
    {
        NotNull(value, paramName);
        if (value.Count > max)
            throw new ArgumentException($"Collection must not exceed {max} items.", paramName);

        return value;
    }

    /// <summary>Throws if the collection has fewer than <paramref name="min"/> items.</summary>
    public static ICollection<T> MinCount<T>(ICollection<T> value, int min, string paramName)
    {
        NotNull(value, paramName);
        if (value.Count < min)
            throw new ArgumentException($"Collection must have at least {min} items.", paramName);

        return value;
    }

    // ── Guid ─────────────────────────────────────────────────────────────────

    /// <summary>Throws if the <see cref="Guid"/> equals <see cref="Guid.Empty"/>.</summary>
    public static Guid NotEmpty(Guid value, string paramName)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Guid must not be empty.", paramName);

        return value;
    }

    // ── Enum ─────────────────────────────────────────────────────────────────

    /// <summary>Throws if the enum value is not defined in its type.</summary>
    public static T Defined<T>(T value, string paramName) where T : struct, Enum
    {
        if (!Enum.IsDefined(value))
            throw new ArgumentException($"Value '{value}' is not a valid {typeof(T).Name}.", paramName);

        return value;
    }

    // ── DateTime ─────────────────────────────────────────────────────────────

    /// <summary>Throws if <paramref name="value"/> equals <see cref="DateTime.MinValue"/> (default).</summary>
    public static DateTime NotDefault(DateTime value, string paramName)
    {
        if (value == default)
            throw new ArgumentException("DateTime must not be the default value.", paramName);

        return value;
    }

    /// <inheritdoc cref="NotDefault(DateTime, string)"/>
    public static DateTimeOffset NotDefault(DateTimeOffset value, string paramName)
    {
        if (value == default)
            throw new ArgumentException("DateTimeOffset must not be the default value.", paramName);

        return value;
    }

    /// <summary>Throws if <paramref name="value"/> is earlier than <see cref="DateTime.UtcNow"/>.</summary>
    public static DateTime NotInPast(DateTime value, string paramName)
    {
        NotDefault(value, paramName);
        if (value < DateTime.UtcNow)
            throw new ArgumentOutOfRangeException(paramName, value, "DateTime must not be in the past.");

        return value;
    }

    /// <summary>Throws if <paramref name="value"/> is later than <see cref="DateTime.UtcNow"/>.</summary>
    public static DateTime NotInFuture(DateTime value, string paramName)
    {
        NotDefault(value, paramName);
        if (value > DateTime.UtcNow)
            throw new ArgumentOutOfRangeException(paramName, value, "DateTime must not be in the future.");

        return value;
    }

    /// <inheritdoc cref="NotInPast(DateTime, string)"/>
    public static DateTimeOffset NotInPast(DateTimeOffset value, string paramName)
    {
        NotDefault(value, paramName);
        if (value < DateTimeOffset.UtcNow)
            throw new ArgumentOutOfRangeException(paramName, value, "DateTimeOffset must not be in the past.");

        return value;
    }

    /// <inheritdoc cref="NotInFuture(DateTime, string)"/>
    public static DateTimeOffset NotInFuture(DateTimeOffset value, string paramName)
    {
        NotDefault(value, paramName);
        if (value > DateTimeOffset.UtcNow)
            throw new ArgumentOutOfRangeException(paramName, value, "DateTimeOffset must not be in the future.");

        return value;
    }

    // ── Format ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Throws if <paramref name="value"/> is null, empty, or not a valid email address.
    /// Returns the trimmed value on success.
    /// </summary>
    public static string Email(string? value, string paramName)
    {
        var trimmed = NotEmpty(value, paramName);
        if (!EmailPattern.IsMatch(trimmed))
            throw new ArgumentException("Value must be a valid email address.", paramName);

        return trimmed;
    }

    /// <summary>
    /// Throws if <paramref name="value"/> is null, empty, or not an absolute HTTP/HTTPS URL.
    /// Returns the trimmed value on success.
    /// </summary>
#pragma warning disable CA1055 // Intentionally returns string to stay consistent with other Check guards
    public static string Url(string? value, string paramName)
#pragma warning restore CA1055
    {
        var trimmed = NotEmpty(value, paramName);
        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("Value must be a valid absolute HTTP or HTTPS URL.", paramName);

        return trimmed;
    }

    /// <summary>
    /// Throws if <paramref name="value"/> is null, empty, or does not match <paramref name="pattern"/>.
    /// Returns the trimmed value on success.
    /// </summary>
    public static string Matches(string? value, string pattern, string paramName)
    {
        ArgumentNullException.ThrowIfNull(pattern);
        var trimmed = NotEmpty(value, paramName);
        try
        {
            if (!Regex.IsMatch(trimmed, pattern, RegexOptions.None, TimeSpan.FromSeconds(1)))
                throw new ArgumentException("Value does not match the required pattern.", paramName);
        }
        catch (RegexMatchTimeoutException)
        {
            throw new ArgumentException("Pattern matching timed out.", paramName);
        }

        return trimmed;
    }
}
