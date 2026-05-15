using CommonUtils.Exceptions;
using CommonUtils.Pagination;

namespace CommonUtils.Tests;

public class PaginationParamsTests
{
    [Fact]
    public void DefaultValues_PageIsOneAndPageSizeIsTwenty()
    {
        var p = new PaginationParams();
        Assert.Equal(1, p.Page);
        Assert.Equal(20, p.PageSize);
    }

    [Fact]
    public void Skip_OnFirstPage_IsZero()
    {
        var p = new PaginationParams { Page = 1, PageSize = 10 };
        Assert.Equal(0, p.Skip);
    }

    [Fact]
    public void Skip_CalculatesCorrectly()
    {
        var p = new PaginationParams { Page = 3, PageSize = 10 };
        Assert.Equal(20, p.Skip);
    }

    [Fact]
    public void Skip_LargePageNumber_CalculatesCorrectly()
    {
        var p = new PaginationParams { Page = 5, PageSize = 25 };
        Assert.Equal(100, p.Skip);
    }

    [Fact]
    public void Take_MatchesPageSize()
    {
        var p = new PaginationParams { Page = 1, PageSize = 25 };
        Assert.Equal(25, p.Take);
    }

    [Fact]
    public void Validate_WithValidParams_DoesNotThrow()
    {
        var p = new PaginationParams { Page = 2, PageSize = 50 };
        var ex = Record.Exception(() => p.Validate());
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WithPageLessThanOne_ThrowsBadRequestException()
    {
        var p = new PaginationParams { Page = 0, PageSize = 10 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_WithNegativePage_ThrowsBadRequestException()
    {
        var p = new PaginationParams { Page = -1, PageSize = 10 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_WithPageSizeZero_ThrowsBadRequestException()
    {
        var p = new PaginationParams { Page = 1, PageSize = 0 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_WithPageSizeNegative_ThrowsBadRequestException()
    {
        var p = new PaginationParams { Page = 1, PageSize = -1 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_WithPageSizeExceedingDefaultMax_ThrowsBadRequestException()
    {
        var p = new PaginationParams { Page = 1, PageSize = 101 };
        Assert.Throws<BadRequestException>(() => p.Validate());
    }

    [Fact]
    public void Validate_WithPageSizeAtDefaultMax_DoesNotThrow()
    {
        var p = new PaginationParams { Page = 1, PageSize = 100 };
        var ex = Record.Exception(() => p.Validate());
        Assert.Null(ex);
    }

    [Fact]
    public void Validate_WithCustomMaxPageSize_ThrowsWhenExceeded()
    {
        var p = new PaginationParams { Page = 1, PageSize = 151 };
        Assert.Throws<BadRequestException>(() => p.Validate(maxPageSize: 150));
    }

    [Fact]
    public void Validate_WithCustomMaxPageSize_AllowsAtExactLimit()
    {
        var p = new PaginationParams { Page = 1, PageSize = 150 };
        var ex = Record.Exception(() => p.Validate(maxPageSize: 150));
        Assert.Null(ex);
    }
}
