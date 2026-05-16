using CommonUtils.Results;

namespace CommonUtils.Tests;

public class ResultTests
{
    // ── Result.Ok<T> ─────────────────────────────────────────────────────────

    [Fact]
    public void Ok_WithValue_IsSuccess()
    {
        Assert.True(Result.Ok(42).IsSuccess);
    }

    [Fact]
    public void Ok_WithValue_SetsValue()
    {
        Assert.Equal(42, Result.Ok(42).Value);
    }

    [Fact]
    public void Ok_WithValue_ErrorsIsEmpty()
    {
        Assert.Empty(Result.Ok(42).Errors);
    }

    [Fact]
    public void Ok_WithValue_IsFailureIsFalse()
    {
        Assert.False(Result.Ok(42).IsFailure);
    }

    [Fact]
    public void Ok_WithStringValue_IsSuccess()
    {
        Assert.True(Result.Ok("hello").IsSuccess);
    }

    // ── Result.Ok() — no value ────────────────────────────────────────────────

    [Fact]
    public void Ok_NoValue_IsSuccess()
    {
        Assert.True(Result.Ok().IsSuccess);
    }

    [Fact]
    public void Ok_NoValue_ValueIsUnitValue()
    {
        Assert.Equal(Unit.Value, Result.Ok().Value);
    }

    // ── Result.Fail<T>(string) ────────────────────────────────────────────────

    [Fact]
    public void Fail_WithSingleError_IsFailure()
    {
        Assert.True(Result.Fail<int>("something went wrong").IsFailure);
    }

    [Fact]
    public void Fail_WithSingleError_IsSuccessIsFalse()
    {
        Assert.False(Result.Fail<int>("something went wrong").IsSuccess);
    }

    [Fact]
    public void Fail_WithSingleError_ExposesError()
    {
        var result = Result.Fail<int>("something went wrong");
        Assert.Single(result.Errors);
        Assert.Equal("something went wrong", result.Errors[0]);
    }

    [Fact]
    public void Fail_WithSingleError_ValueIsDefault()
    {
        Assert.Equal(0, Result.Fail<int>("error").Value);
    }

    // ── Result.Fail<T>(IEnumerable<string>) ───────────────────────────────────

    [Fact]
    public void Fail_WithMultipleErrors_ExposesAllErrors()
    {
        var result = Result.Fail<string>(["error one", "error two"]);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains("error one", result.Errors);
        Assert.Contains("error two", result.Errors);
    }

    // ── Result.Fail(string) — no value ────────────────────────────────────────

    [Fact]
    public void Fail_NoValue_IsFailure()
    {
        Assert.True(Result.Fail("something went wrong").IsFailure);
    }

    [Fact]
    public void Fail_NoValue_ExposesError()
    {
        var result = Result.Fail("something went wrong");
        Assert.Single(result.Errors);
        Assert.Equal("something went wrong", result.Errors[0]);
    }

    // ── Result.Fail(IEnumerable<string>) — no value ───────────────────────────

    [Fact]
    public void Fail_NoValue_MultipleErrors_ExposesAllErrors()
    {
        var result = Result.Fail(["err1", "err2"]);
        Assert.Equal(2, result.Errors.Count);
    }

    // ── Implicit operator T → Result<T> ──────────────────────────────────────

    [Fact]
    public void ImplicitOperator_FromValue_IsSuccess()
    {
        Result<int> result = 99;
        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value);
    }

    [Fact]
    public void ImplicitOperator_FromObject_IsSuccess()
    {
        Result<string> result = "hello";
        Assert.True(result.IsSuccess);
        Assert.Equal("hello", result.Value);
    }

    // ── Unit ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Unit_Value_EqualsDefault()
    {
        Assert.Equal(default, Unit.Value);
    }

    [Fact]
    public void Unit_TwoInstances_AreEqual()
    {
        Assert.Equal(Unit.Value, new Unit());
    }
}
