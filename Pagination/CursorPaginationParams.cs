using CommonUtils.Exceptions;

namespace CommonUtils.Pagination;

/// <summary>
/// Cursor/keyset pagination parameters. Use instead of offset pagination when working
/// with large datasets — cursor pagination avoids the performance degradation of
/// <c>OFFSET n</c> at high page numbers.
/// </summary>
/// <typeparam name="TCursor">
/// The type of the cursor value (e.g. <see cref="int"/>, <see cref="Guid"/>,
/// <see cref="DateTime"/>). Must be equatable and orderable in the underlying store.
/// </typeparam>
public record CursorPaginationParams<TCursor>
{
    private const int DefaultPageSize = 20;
    private const int DefaultMaxPageSize = 100;

    /// <summary>
    /// The cursor value from the previous page's <see cref="CursorPagedResult{T,TCursor}.NextCursor"/>.
    /// Pass <c>null</c> to start from the beginning.
    /// </summary>
    public TCursor? After { get; init; }

    /// <summary>Number of items to return. Defaults to 20.</summary>
    public int PageSize { get; init; } = DefaultPageSize;

    /// <summary>
    /// Validates that <see cref="PageSize"/> is within [1, <paramref name="maxPageSize"/>].
    /// Throws <see cref="BadRequestException"/> on failure.
    /// </summary>
    public void Validate(int maxPageSize = DefaultMaxPageSize)
    {
        if (PageSize < 1)
            throw new BadRequestException($"'{nameof(PageSize)}' must be 1 or greater.");

        if (PageSize > maxPageSize)
            throw new BadRequestException($"'{nameof(PageSize)}' must not exceed {maxPageSize}.");
    }
}

/// <summary>
/// A page of items from a cursor-based query, with the cursor needed to fetch the next page.
/// </summary>
/// <typeparam name="T">Item type.</typeparam>
/// <typeparam name="TCursor">Cursor type.</typeparam>
public record CursorPagedResult<T, TCursor>
{
    /// <summary>The items on the current page.</summary>
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// The cursor to pass as <c>after</c> on the next request.
    /// <c>null</c> when there are no more pages.
    /// </summary>
    public TCursor? NextCursor { get; init; }

    /// <summary><c>true</c> when there is at least one more page available.</summary>
    public bool HasNextPage => NextCursor is not null;

    /// <summary>Number of items returned on this page.</summary>
    public int Count => Items.Count;
}

/// <summary>
/// Non-generic factory for <see cref="CursorPagedResult{T,TCursor}"/>.
/// </summary>
public static class CursorPagedResult
{
    /// <summary>
    /// Creates a <see cref="CursorPagedResult{T,TCursor}"/> from a list of items.
    /// Pass <paramref name="nextCursor"/> as <c>null</c> when there is no next page.
    /// </summary>
    public static CursorPagedResult<T, TCursor> Create<T, TCursor>(
        IReadOnlyList<T> items,
        TCursor? nextCursor)
    {
        ArgumentNullException.ThrowIfNull(items);
        return new CursorPagedResult<T, TCursor>
        {
            Items = items,
            NextCursor = nextCursor
        };
    }

    /// <summary>Creates an empty result with no next page.</summary>
    public static CursorPagedResult<T, TCursor> Empty<T, TCursor>() =>
        new() { Items = [], NextCursor = default };
}
