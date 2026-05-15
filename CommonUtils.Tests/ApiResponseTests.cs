using CommonUtils.Responses;

namespace CommonUtils.Tests;

public class ApiResponseTests
{
    // ── Non-generic (no-data) ─────────────────────────────────────────────────

    [Fact]
    public void Ok_NoArgs_ReturnsSuccessWithNoMessageAndNoErrors()
    {
        var response = ApiResponse.Ok();
        Assert.True(response.Success);
        Assert.Null(response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void Ok_WithMessage_ReturnsSuccessWithMessage()
    {
        var response = ApiResponse.Ok("Created successfully.");
        Assert.True(response.Success);
        Assert.Equal("Created successfully.", response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void Fail_WithSingleError_ReturnsFailureWithOneError()
    {
        var response = ApiResponse.Fail("Not found");
        Assert.False(response.Success);
        Assert.Single(response.Errors);
        Assert.Equal("Not found", response.Errors[0]);
    }

    [Fact]
    public void Fail_WithMultipleErrors_ReturnsFailureWithAllErrors()
    {
        var errors = new[] { "Error A", "Error B" };
        var response = ApiResponse.Fail(errors);
        Assert.False(response.Success);
        Assert.Equal(2, response.Errors.Count);
        Assert.Contains("Error A", response.Errors);
        Assert.Contains("Error B", response.Errors);
    }

    [Fact]
    public void Fail_WithMultipleErrors_MessageIsNull()
    {
        var response = ApiResponse.Fail(["e1", "e2"]);
        Assert.Null(response.Message);
    }

    // ── Generic (with-data) ───────────────────────────────────────────────────

    [Fact]
    public void OkGeneric_WithData_ReturnsSuccessWithDataAndNoErrors()
    {
        var response = ApiResponse.Ok(42);
        Assert.True(response.Success);
        Assert.Equal(42, response.Data);
        Assert.Null(response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void OkGeneric_WithDataAndMessage_ReturnsBothDataAndMessage()
    {
        var response = ApiResponse.Ok("hello", "Fetched successfully.");
        Assert.True(response.Success);
        Assert.Equal("hello", response.Data);
        Assert.Equal("Fetched successfully.", response.Message);
        Assert.Empty(response.Errors);
    }

    [Fact]
    public void OkGeneric_WithReferenceTypeData_ReturnsSameInstance()
    {
        var data = new List<int> { 1, 2, 3 };
        var response = ApiResponse.Ok(data);
        Assert.Same(data, response.Data);
    }

    [Fact]
    public void FailGeneric_WithSingleError_ReturnsFailureWithError()
    {
        var response = ApiResponse.Fail<int>("Oops");
        Assert.False(response.Success);
        Assert.Single(response.Errors);
        Assert.Equal("Oops", response.Errors[0]);
        Assert.Equal(default, response.Data);
    }

    [Fact]
    public void FailGeneric_WithMultipleErrors_ReturnsAllErrors()
    {
        var response = ApiResponse.Fail<string>(["Err A", "Err B"]);
        Assert.False(response.Success);
        Assert.Equal(2, response.Errors.Count);
        Assert.Contains("Err A", response.Errors);
        Assert.Contains("Err B", response.Errors);
    }

    [Fact]
    public void FailGeneric_DataIsDefaultValue()
    {
        var response = ApiResponse.Fail<string>("error");
        Assert.Null(response.Data);
    }
}
