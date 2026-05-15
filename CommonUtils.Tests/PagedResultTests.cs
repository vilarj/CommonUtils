using CommonUtils.Pagination;

namespace CommonUtils.Tests;

public class PagedResultTests
{
    private static PaginationParams Params(int page = 1, int pageSize = 10) =>
        new() { Page = page, PageSize = pageSize };

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public void Create_WithValidInputs_SetsAllFields()
    {
        var items = new List<string> { "a", "b", "c" }.AsReadOnly();
        var result = PagedResult.Create(items, 30, Params(2, 10));

        Assert.Same(items, result.Items);
        Assert.Equal(2, result.Page);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(30, result.TotalCount);
    }

    [Fact]
    public void Create_WithNullItems_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PagedResult.Create<string>(null!, 0, Params()));
    }

    [Fact]
    public void Create_WithNullPagination_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PagedResult.Create<string>([], 0, null!));
    }

    // ── Empty ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Empty_ReturnsNoItemsAndZeroTotalCount()
    {
        var result = PagedResult.Empty<string>(Params());
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }

    [Fact]
    public void Empty_SetsPageAndPageSizeFromParams()
    {
        var result = PagedResult.Empty<string>(Params(3, 25));
        Assert.Equal(3, result.Page);
        Assert.Equal(25, result.PageSize);
    }

    [Fact]
    public void Empty_WithNullPagination_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => PagedResult.Empty<string>(null!));
    }

    // ── TotalPages ────────────────────────────────────────────────────────────

    [Fact]
    public void TotalPages_WithExactDivision_ReturnsExactPageCount()
    {
        var result = PagedResult.Create<int>([], 20, Params(1, 10));
        Assert.Equal(2, result.TotalPages);
    }

    [Fact]
    public void TotalPages_WithRemainder_CeilsUp()
    {
        var result = PagedResult.Create<int>([], 25, Params(1, 10));
        Assert.Equal(3, result.TotalPages);
    }

    [Fact]
    public void TotalPages_WithZeroTotalCount_ReturnsZero()
    {
        var result = PagedResult.Create<int>([], 0, Params(1, 10));
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void TotalPages_WithPageSizeZero_ReturnsZero()
    {
        var result = new PagedResult<int>
        {
            Items = [],
            Page = 1,
            PageSize = 0,
            TotalCount = 10
        };
        Assert.Equal(0, result.TotalPages);
    }

    [Fact]
    public void TotalPages_WithSingleItem_ReturnsOne()
    {
        var result = PagedResult.Create<int>([], 1, Params(1, 10));
        Assert.Equal(1, result.TotalPages);
    }

    // ── HasNextPage ───────────────────────────────────────────────────────────

    [Fact]
    public void HasNextPage_WhenMorePagesExist_ReturnsTrue()
    {
        var result = PagedResult.Create<int>([], 30, Params(2, 10)); // page 2 of 3
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WhenOnLastPage_ReturnsFalse()
    {
        var result = PagedResult.Create<int>([], 30, Params(3, 10)); // page 3 of 3
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WhenOnOnlyPage_ReturnsFalse()
    {
        var result = PagedResult.Create<int>([], 5, Params(1, 10)); // 1 page total
        Assert.False(result.HasNextPage);
    }

    // ── HasPreviousPage ───────────────────────────────────────────────────────

    [Fact]
    public void HasPreviousPage_WhenOnFirstPage_ReturnsFalse()
    {
        var result = PagedResult.Create<int>([], 30, Params(1, 10));
        Assert.False(result.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_WhenNotOnFirstPage_ReturnsTrue()
    {
        var result = PagedResult.Create<int>([], 30, Params(2, 10));
        Assert.True(result.HasPreviousPage);
    }

    [Fact]
    public void HasPreviousPage_WhenOnLastPage_ReturnsTrue()
    {
        var result = PagedResult.Create<int>([], 30, Params(3, 10));
        Assert.True(result.HasPreviousPage);
    }
}
