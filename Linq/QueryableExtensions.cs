using System.Linq.Expressions;
using CommonUtils.Pagination;

namespace CommonUtils.Linq;

/// <summary>
/// <see cref="IQueryable{T}"/> extensions that apply pagination and sorting without
/// coupling to any specific ORM. The async materializer overload accepts a delegate
/// so callers can pass EF Core's <c>ToListAsync</c> without a hard dependency.
/// </summary>
public static class QueryableExtensions
{

    /// <summary>
    /// Applies <see cref="PaginationParams.Skip"/> and <see cref="PaginationParams.Take"/>
    /// to the query. Does not validate the params — call <see cref="PaginationParams.Validate"/>
    /// before this if needed.
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, PaginationParams pagination)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(pagination);
        return query.Skip(pagination.Skip).Take(pagination.Take);
    }
    

    /// <summary>
    /// Applies an <c>OrderBy</c> / <c>OrderByDescending</c> based on <paramref name="sort"/>
    /// using a caller-supplied map of allowed column names to key-selector expressions.
    /// When <see cref="SortParams.IsActive"/> is <c>false</c> the query is returned unchanged.
    /// </summary>
    /// <param name="query">The source query.</param>
    /// <param name="sort">Sort parameters bound from the query string.</param>
    /// <param name="columnMap">
    /// Dictionary mapping allowed column names (case-insensitive) to key-selector expressions.
    /// Example: <c>{ ["name"] = x => x.Name, ["createdAt"] = x => x.CreatedAt }</c>
    /// </param>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        SortParams sort,
        IReadOnlyDictionary<string, Expression<Func<T, object?>>> columnMap)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(sort);
        ArgumentNullException.ThrowIfNull(columnMap);

        if (!sort.IsActive)
            return query;

        var key = columnMap.Keys.FirstOrDefault(k => k.Equals(sort.SortBy, StringComparison.OrdinalIgnoreCase));
        if (key is null)
            return query;

        var selector = columnMap[key];
        return sort.Direction == SortDirection.Desc
            ? query.OrderByDescending(selector)
            : query.OrderBy(selector);
    }
    

    /// <summary>
    /// Counts all matching items, then fetches one page and wraps everything in a
    /// <see cref="PagedResult{T}"/>. The <paramref name="materialize"/> delegate
    /// should be <c>q => q.ToListAsync(ct)</c> (EF Core) or any equivalent async
    /// materializer — keeps this library free of an EF Core dependency.
    /// </summary>
    /// <param name="query">The pre-filtered, pre-sorted query (without Skip/Take).</param>
    /// <param name="pagination">Pagination parameters.</param>
    /// <param name="materialize">
    /// Async delegate that converts an <see cref="IQueryable{T}"/> to a list.
    /// </param>
    /// <param name="countAsync">
    /// Async delegate that returns the total count for the query.
    /// </param>
    /// <param name="cancellationToken">Propagated to both delegates.</param>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PaginationParams pagination,
        Func<IQueryable<T>, CancellationToken, Task<List<T>>> materialize,
        Func<IQueryable<T>, CancellationToken, Task<int>> countAsync,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);
        ArgumentNullException.ThrowIfNull(pagination);
        ArgumentNullException.ThrowIfNull(materialize);
        ArgumentNullException.ThrowIfNull(countAsync);

        var total = await countAsync(query, cancellationToken).ConfigureAwait(false);

        if (total == 0)
            return PagedResult.Empty<T>(pagination);

        var items = await materialize(
            query.Skip(pagination.Skip).Take(pagination.Take),
            cancellationToken).ConfigureAwait(false);

        return PagedResult.Create(items.AsReadOnly(), total, pagination);
    }
}
