using CommonUtils.Exceptions;

namespace CommonUtils.Pagination;

/// <summary>
/// A single sort criterion — a column name paired with a direction.
/// </summary>
public record SortCriterion
{
    /// <summary>Column name to sort by.</summary>
    public string SortBy { get; init; } = string.Empty;

    /// <summary>Sort direction. Defaults to ascending.</summary>
    public SortDirection Direction { get; init; } = SortDirection.Asc;
}

/// <summary>
/// Multi-column sort parameters for list endpoints that support compound ordering.
/// Bind from the query string as a repeated parameter:
/// <c>?sort=name:asc&amp;sort=createdAt:desc</c>
/// </summary>
/// <remarks>
/// Each item in <see cref="Criteria"/> is a <c>column:direction</c> string, for example
/// <c>"name:asc"</c> or <c>"createdAt:desc"</c>. Direction is optional and defaults to
/// ascending when omitted.
/// </remarks>
public record MultiSortParams
{
    /// <summary>Raw sort strings from the query string (e.g. <c>"name:asc"</c>).</summary>
    public IReadOnlyList<string> Sort { get; init; } = [];

    /// <summary>
    /// Parsed sort criteria. Available after calling <see cref="Validate"/>.
    /// </summary>
    public IReadOnlyList<SortCriterion> Criteria { get; private set; } = [];

    /// <summary><c>true</c> when at least one sort criterion is present.</summary>
    public bool IsActive => Sort.Count > 0;

    /// <summary>
    /// Parses and validates each sort string, ensuring column names are in
    /// <paramref name="allowedColumns"/> (case-insensitive). Populates
    /// <see cref="Criteria"/> on success.
    /// Throws <see cref="BadRequestException"/> on the first invalid value.
    /// </summary>
    public MultiSortParams Validate(IEnumerable<string> allowedColumns)
    {
        ArgumentNullException.ThrowIfNull(allowedColumns);
        var allowed = allowedColumns.ToList();
        var parsed = new List<SortCriterion>(Sort.Count);

        foreach (var entry in Sort)
        {
            var parts = entry.Split(':', 2, StringSplitOptions.TrimEntries);
            var column = parts[0];

            if (!allowed.Any(c => c.Equals(column, StringComparison.OrdinalIgnoreCase)))
                throw new BadRequestException($"'{column}' is not a valid sort column.");

            SortDirection direction;
            if (parts.Length == 1 || string.IsNullOrWhiteSpace(parts[1]))
            {
                direction = SortDirection.Asc;
            }
            else if (parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
            {
                direction = SortDirection.Desc;
            }
            else if (parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase))
            {
                direction = SortDirection.Asc;
            }
            else
            {
                throw new BadRequestException($"'{parts[1]}' is not a valid sort direction. Use 'asc' or 'desc'.");
            }

            parsed.Add(new SortCriterion { SortBy = column, Direction = direction });
        }

        Criteria = parsed.AsReadOnly();
        return this;
    }
}
