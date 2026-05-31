using CommonUtils.Exceptions;
using CommonUtils.Pagination;

namespace CommonUtils.Tests;

public class CursorPaginationTests
{
    // ── CursorPaginationParams.Validate ───────────────────────────────────────

    [Fact]
    public void Validate_DefaultPageSize_Passes()
    {
        var p = new CursorPaginationParams<int>();
        var ex = Record.Exception(() => p.Validate());
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_PageSizeAboveMax_ThrowsBadRequestException()
    {
        var p = new CursorPaginationParams<int> { PageSize = 200 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_PageSizeZero_ThrowsBadRequestException()
    {
        var p = new CursorPaginationParams<int> { PageSize = 0 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_CustomMaxPageSize_IsRespected()
    {
        var p = new CursorPaginationParams<int> { PageSize = 50 };
        Assert.Throws<BadRequestException>(() => p.Validate(maxPageSize: 25));
    }

    [Fact]
    public void Validate_ExactlyAtMax_Passes()
    {
        var p = new CursorPaginationParams<int> { PageSize = 100 };
        var ex = Record.Exception(() => p.Validate(100));
        Assert.Null(ex);
    }

    [Fact]
    public void After_DefaultsToNull()
    {
        var p = new CursorPaginationParams<int?>();
        Assert.Null(p.After);
    }

    // ── CursorPagedResult<T,TCursor> ──────────────────────────────────────────

    [Fact]
    public void HasNextPage_WhenNextCursorIsNull_ReturnsFalse()
    {
        var result = new CursorPagedResult<string, string?>
        {
            Items = ["a", "b"],
            NextCursor = null
        };
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void HasNextPage_WhenNextCursorIsNotNull_ReturnsTrue()
    {
        var result = new CursorPagedResult<string, string?>
        {
            Items = ["a", "b"],
            NextCursor = "cursor-xyz"
        };
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void Count_ReturnsItemCount()
    {
        var result = new CursorPagedResult<int, int?>
        {
            Items = [1, 2, 3],
            NextCursor = null
        };
        Assert.Equal(3, result.Count);
    }

    // ── CursorPagedResult (factory) ───────────────────────────────────────────

    [Fact]
    public void Create_SetsItemsAndNextCursor()
    {
        var items = new List<int> { 10, 20 }.AsReadOnly();
        var result = CursorPagedResult.Create(items, 21);
        Assert.Equal(items, result.Items);
        Assert.Equal(21, result.NextCursor);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void Create_WithNullNextCursor_HasNextPageFalse()
    {
        var items = new List<string> { "x" }.AsReadOnly();
        var result = CursorPagedResult.Create<string, string?>(items, null);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void Create_NullItems_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            CursorPagedResult.Create<int, int?>(null!, null));
    }

    [Fact]
    public void Empty_ReturnsEmptyResultWithNoNextPage()
    {
        var result = CursorPagedResult.Empty<string, string?>();
        Assert.Empty(result.Items);
        Assert.Null(result.NextCursor);
        Assert.False(result.HasNextPage);
    }
}
