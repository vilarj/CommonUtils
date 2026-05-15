namespace CommonUtils.Pagination;

/// <summary>
/// Standard pagination parameters for any list endpoint.
/// Bind directly from query string: <c>?page=2&amp;pageSize=50</c>
/// </summary>
public record PaginationParams
{
    private const int DefaultPageSize = 20;
    private const int DefaultMaxPageSize = 100;

    /// <summary>1-based page number. Defaults to 1.</summary>
    public int Page { get; init; } = 1;

    /// <summary>Number of items per page. Defaults to 20.</summary>
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>Number of items to skip — pass directly to EF Core / Dapper / LINQ.</summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>Alias for <see cref="PageSize"/> — pass directly to <c>.Take()</c>.</summary>
    public int Take => PageSize;

    /// <summary>
    /// Validates that <see cref="Page"/> ≥ 1 and <see cref="PageSize"/> is within [1, <paramref name="maxPageSize"/>].
    /// Throws <see cref="Exceptions.BadRequestException"/> on failure so the caller doesn't need a try/catch.
    /// </summary>
    public void Validate(int maxPageSize = DefaultMaxPageSize)
    {
        if (Page < 1)
            throw new Exceptions.BadRequestException($"'{nameof(Page)}' must be 1 or greater.");

        if (PageSize < 1)
            throw new Exceptions.BadRequestException($"'{nameof(PageSize)}' must be 1 or greater.");

        if (PageSize > maxPageSize)
            throw new Exceptions.BadRequestException($"'{nameof(PageSize)}' must not exceed {maxPageSize}.");
    }
}
