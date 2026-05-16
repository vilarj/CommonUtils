using CommonUtils.Exceptions;
using CommonUtils.Pagination;

namespace CommonUtils.Tests;

public class SortParamsTests
{
    private static readonly string[] AllowedColumns = ["name", "createdAt", "price"];

    // ── Defaults ─────────────────────────────────────────────────────────────

    [Fact]
    public void DefaultValues_SortByIsNullAndDirectionIsAsc()
    {
        var p = new SortParams();
        Assert.Null(p.SortBy);
        Assert.Equal(SortDirection.Asc, p.Direction);
    }

    // ── IsActive ─────────────────────────────────────────────────────────────

    [Fact]
    public void IsActive_WhenSortByIsNull_ReturnsFalse()
    {
        Assert.False(new SortParams().IsActive);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void IsActive_WhenSortByIsEmptyOrWhitespace_ReturnsFalse(string sortBy)
    {
        Assert.False(new SortParams { SortBy = sortBy }.IsActive);
    }

    [Fact]
    public void IsActive_WhenSortByIsSet_ReturnsTrue()
    {
        Assert.True(new SortParams { SortBy = "name" }.IsActive);
    }

    // ── Validate ─────────────────────────────────────────────────────────────

    [Fact]
    public void Validate_WhenSortByIsNull_DoesNotThrow()
    {
        var ex = Record.Exception(() => new SortParams().Validate(AllowedColumns));
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WithAllowedColumn_DoesNotThrow()
    {
        var ex = Record.Exception(() => new SortParams { SortBy = "name" }.Validate(AllowedColumns));
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WithAllowedColumn_IsCaseInsensitive()
    {
        var ex = Record.Exception(() => new SortParams { SortBy = "CREATEDAT" }.Validate(AllowedColumns));
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WithDisallowedColumn_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() =>
            new SortParams { SortBy = "secretField" }.Validate(AllowedColumns));
    }

    [Fact]
    public void Validate_WithEmptyAllowedColumns_ThrowsBadRequestException()
    {
        Assert.Throws<BadRequestException>(() =>
            new SortParams { SortBy = "name" }.Validate([]));
    }

    // ── Direction ─────────────────────────────────────────────────────────────

    [Fact]
    public void Direction_CanBeSetToDesc()
    {
        var p = new SortParams { SortBy = "price", Direction = SortDirection.Desc };
        Assert.Equal(SortDirection.Desc, p.Direction);
    }
}
