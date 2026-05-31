using System.Linq.Expressions;
using CommonUtils.Linq;
using CommonUtils.Pagination;

namespace CommonUtils.Tests;

public class QueryableExtensionsTests
{
    private static IQueryable<int> MakeQuery(IEnumerable<int> source) => source.AsQueryable();

    // ── ApplyPagination ───────────────────────────────────────────────────────

    [Fact]
    public void ApplyPagination_FirstPage_ReturnsCorrectItems()
    {
        var query = MakeQuery(Enumerable.Range(1, 50));
        var pagination = new PaginationParams { Page = 1, PageSize = 10 };
        var result = query.ApplyPagination(pagination).ToList();
        Assert.Equal(Enumerable.Range(1, 10), result);
    }

    [Fact]
    public void ApplyPagination_SecondPage_SkipsCorrectly()
    {
        var query = MakeQuery(Enumerable.Range(1, 50));
        var pagination = new PaginationParams { Page = 2, PageSize = 5 };
        var result = query.ApplyPagination(pagination).ToList();
        Assert.Equal(Enumerable.Range(6, 5), result);
    }

    [Fact]
    public void ApplyPagination_NullQuery_ThrowsArgumentNullException()
    {
        IQueryable<int> query = null!;
        Assert.Throws<ArgumentNullException>(() =>
            query.ApplyPagination(new PaginationParams()));
    }

    [Fact]
    public void ApplyPagination_NullPagination_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            MakeQuery([1, 2]).ApplyPagination(null!));
    }

    // ── ApplySorting ──────────────────────────────────────────────────────────

    private record Item(string Name, int Age);

    [Fact]
    public void ApplySorting_WhenNotActive_ReturnsQueryUnchanged()
    {
        var items = new[] { new Item("B", 2), new Item("A", 1) }.AsQueryable();
        var sort = new SortParams(); // IsActive = false
        var map = new Dictionary<string, Expression<Func<Item, object?>>>
        {
            ["name"] = x => x.Name
        };
        var result = items.ApplySorting(sort, map).ToList();
        Assert.Equal("B", result[0].Name);
    }

    [Fact]
    public void ApplySorting_AscDirection_OrdersAscending()
    {
        var items = new[] { new Item("C", 3), new Item("A", 1), new Item("B", 2) }.AsQueryable();
        var sort = new SortParams { SortBy = "name", Direction = SortDirection.Asc };
        var map = new Dictionary<string, Expression<Func<Item, object?>>>
        {
            ["name"] = x => x.Name
        };
        var result = items.ApplySorting(sort, map).ToList();
        Assert.Equal(["A", "B", "C"], result.Select(i => i.Name));
    }

    [Fact]
    public void ApplySorting_DescDirection_OrdersDescending()
    {
        var items = new[] { new Item("A", 1), new Item("C", 3), new Item("B", 2) }.AsQueryable();
        var sort = new SortParams { SortBy = "name", Direction = SortDirection.Desc };
        var map = new Dictionary<string, Expression<Func<Item, object?>>>
        {
            ["name"] = x => x.Name
        };
        var result = items.ApplySorting(sort, map).ToList();
        Assert.Equal(["C", "B", "A"], result.Select(i => i.Name));
    }

    [Fact]
    public void ApplySorting_UnknownColumn_ReturnsQueryUnchanged()
    {
        var items = new[] { new Item("B", 2), new Item("A", 1) }.AsQueryable();
        var sort = new SortParams { SortBy = "unknown" };
        var map = new Dictionary<string, Expression<Func<Item, object?>>>
        {
            ["name"] = x => x.Name
        };
        var result = items.ApplySorting(sort, map).ToList();
        Assert.Equal("B", result[0].Name);
    }

    [Fact]
    public void ApplySorting_CaseInsensitiveColumnMatch()
    {
        var items = new[] { new Item("B", 2), new Item("A", 1) }.AsQueryable();
        var sort = new SortParams { SortBy = "NAME", Direction = SortDirection.Asc };
        var map = new Dictionary<string, Expression<Func<Item, object?>>>
        {
            ["name"] = x => x.Name
        };
        var result = items.ApplySorting(sort, map).ToList();
        Assert.Equal("A", result[0].Name);
    }

    // ── ToPagedResultAsync ────────────────────────────────────────────────────

    [Fact]
    public async Task ToPagedResultAsync_ReturnsCorrectPage()
    {
        var query = Enumerable.Range(1, 100).AsQueryable();
        var pagination = new PaginationParams { Page = 2, PageSize = 10 };

        var result = await query.ToPagedResultAsync(
            pagination,
            (q, _) => Task.FromResult(q.ToList()),
            (q, _) => Task.FromResult(q.Count()),
            CancellationToken.None);

        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(100, result.TotalCount);
        Assert.Equal(10, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.True(result.HasPreviousPage);
        Assert.Equal(Enumerable.Range(11, 10), result.Items);
    }

    [Fact]
    public async Task ToPagedResultAsync_EmptySource_ReturnsEmptyResult()
    {
        var query = Enumerable.Empty<int>().AsQueryable();
        var pagination = new PaginationParams();

        var result = await query.ToPagedResultAsync(
            pagination,
            (q, _) => Task.FromResult(q.ToList()),
            (q, _) => Task.FromResult(q.Count()),
            CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.False(result.HasNextPage);
        Assert.False(result.HasPreviousPage);
    }
}
