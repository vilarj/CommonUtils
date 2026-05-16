namespace CommonUtils.Pagination;

/// <summary>
/// Standard sort parameters for list endpoints.
/// Bind directly from query string: <c>?sortBy=createdAt&amp;direction=Desc</c>
/// </summary>
public record SortParams
{
    /// <summary>Column name to sort by. Null or empty means no explicit ordering is applied.</summary>
    public string? SortBy { get; init; }

    /// <summary>Sort direction. Defaults to ascending.</summary>
    public SortDirection Direction { get; init; } = SortDirection.Asc;

    /// <summary>True when <see cref="SortBy"/> is a non-empty string.</summary>
    public bool IsActive => !string.IsNullOrWhiteSpace(SortBy);

    /// <summary>
    /// Validates that <see cref="SortBy"/>, when set, is one of the <paramref name="allowedColumns"/> (case-insensitive).
    /// Throws <see cref="Exceptions.BadRequestException"/> on failure so the caller doesn't need a try/catch.
    /// No-op when <see cref="IsActive"/> is false.
    /// </summary>
    public void Validate(IEnumerable<string> allowedColumns)
    {
        if (!IsActive)
            return;

        if (!allowedColumns.Any(c => c.Equals(SortBy, StringComparison.OrdinalIgnoreCase)))
            throw new Exceptions.BadRequestException($"'{SortBy}' is not a valid sort column.");
    }
}
