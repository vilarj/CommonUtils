using CommonUtils.Exceptions;
using CommonUtils.Pagination;

namespace CommonUtils.Tests;

public class MultiSortParamsTests
{
    // ── IsActive ──────────────────────────────────────────────────────────────

    [Fact]
    public void IsActive_WithNoSort_ReturnsFalse()
    {
        var p = new MultiSortParams();
        Assert.False(p.IsActive);
    }

    [Fact]
    public void IsActive_WithSortEntries_ReturnsTrue()
    {
        var p = new MultiSortParams { Sort = ["name:asc"] };
        Assert.True(p.IsActive);
    }

    // ── Validate — happy path ─────────────────────────────────────────────────

    [Fact]
    public void Validate_SingleAsc_ParsesCorrectly()
    {
        var p = new MultiSortParams { Sort = ["name:asc"] };
        p.Validate(["name", "age"]);
        Assert.Single(p.Criteria);
        Assert.Equal("name", p.Criteria[0].SortBy);
        Assert.Equal(SortDirection.Asc, p.Criteria[0].Direction);
    }

    [Fact]
    public void Validate_SingleDesc_ParsesCorrectly()
    {
        var p = new MultiSortParams { Sort = ["createdAt:desc"] };
        p.Validate(["createdAt"]);
        Assert.Equal(SortDirection.Desc, p.Criteria[0].Direction);
    }

    [Fact]
    public void Validate_DirectionOmitted_DefaultsToAsc()
    {
        var p = new MultiSortParams { Sort = ["name"] };
        p.Validate(["name"]);
        Assert.Equal(SortDirection.Asc, p.Criteria[0].Direction);
    }

    [Fact]
    public void Validate_MultipleColumns_ParsesAll()
    {
        var p = new MultiSortParams { Sort = ["name:asc", "age:desc"] };
        p.Validate(["name", "age"]);
        Assert.Equal(2, p.Criteria.Count);
        Assert.Equal("name", p.Criteria[0].SortBy);
        Assert.Equal("age", p.Criteria[1].SortBy);
    }

    [Fact]
    public void Validate_CaseInsensitiveColumn_Passes()
    {
        var p = new MultiSortParams { Sort = ["NAME:asc"] };
        p.Validate(["name"]);
        Assert.Equal("NAME", p.Criteria[0].SortBy);
    }

    [Fact]
    public void Validate_EmptySort_ProducesEmptyCriteria()
    {
        var p = new MultiSortParams();
        p.Validate(["name"]);
        Assert.Empty(p.Criteria);
    }

    // ── Validate — error cases ────────────────────────────────────────────────

    [Fact]
    public void Validate_UnknownColumn_ThrowsBadRequestException()
    {
        var p = new MultiSortParams { Sort = ["unknown:asc"] };
        Assert.Throws<BadRequestException>(() => p.Validate(["name"]));
    }

    [Fact]
    public void Validate_InvalidDirection_ThrowsBadRequestException()
    {
        var p = new MultiSortParams { Sort = ["name:sideways"] };
        Assert.Throws<BadRequestException>(() => p.Validate(["name"]));
    }

    [Fact]
    public void Validate_NullAllowedColumns_ThrowsArgumentNullException()
    {
        var p = new MultiSortParams { Sort = ["name:asc"] };
        Assert.Throws<ArgumentNullException>(() => p.Validate(null!));
    }

    [Fact]
    public void Validate_ReturnsThis_ForFluentChaining()
    {
        var p = new MultiSortParams { Sort = ["name:asc"] };
        var returned = p.Validate(["name"]);
        Assert.Same(p, returned);
    }
}
