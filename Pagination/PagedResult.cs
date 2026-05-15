namespace CommonUtils.Pagination;

/// <summary>
/// Wraps a page of items with metadata that clients need to build pagination UI.
/// Use <see cref="PagedResult.Create{T}"/> or <see cref="PagedResult.Empty{T}"/>
/// to construct instances.
/// </summary>
public record PagedResult<T>
{
    /// <summary>The items on the current page.</summary>
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>Current page number (1-based).</summary>
    public int Page { get; init; }

    /// <summary>Maximum items per page.</summary>
    public int PageSize { get; init; }

    /// <summary>Total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>Total number of pages.</summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

    /// <summary>Whether there is a page after the current one.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>Whether there is a page before the current one.</summary>
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// Non-generic factory for <see cref="PagedResult{T}"/>.
/// Keeps static members off the generic type (CA1000) and allows
/// type inference: <c>PagedResult.Create(items, total, pagination)</c>
/// </summary>
public static class PagedResult
{
    /// <summary>Creates a populated result from a list of items and pagination params.</summary>
    public static PagedResult<T> Create<T>(IReadOnlyList<T> items, int totalCount, PaginationParams pagination)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(pagination);

        return new PagedResult<T>
        {
            Items = items,
            Page = pagination.Page,
            PageSize = pagination.PageSize,
            TotalCount = totalCount
        };
    }

    /// <summary>Creates an empty result (no items, zero total) for the given pagination params.</summary>
    public static PagedResult<T> Empty<T>(PaginationParams pagination)
    {
        ArgumentNullException.ThrowIfNull(pagination);
        return Create<T>([], 0, pagination);
    }
}
