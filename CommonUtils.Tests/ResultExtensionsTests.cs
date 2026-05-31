using CommonUtils.Responses;
using CommonUtils.Results;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Tests;

public class ResultExtensionsTests
{
    // ── Map ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Map_OnSuccess_ProjectsValue()
    {
        var result = Result.Ok(42);
        var mapped = result.Map(x => x.ToString());
        Assert.True(mapped.IsSuccess);
        Assert.Equal("42", mapped.Value);
    }

    [Fact]
    public void Map_OnFailure_ForwardsErrors()
    {
        var result = Result.Fail<int>("error");
        var mapped = result.Map(x => x.ToString());
        Assert.True(mapped.IsFailure);
        Assert.Contains("error", mapped.Errors);
    }

    [Fact]
    public void Map_NullResult_ThrowsArgumentNullException()
    {
        Result<int> result = null!;
        Assert.Throws<ArgumentNullException>(() => result.Map(x => x.ToString()));
    }

    [Fact]
    public void Map_NullMap_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Result.Ok(1).Map<int, string>(null!));
    }

    // ── Bind ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Bind_OnSuccess_InvokesBind()
    {
        var result = Result.Ok(5);
        var bound = result.Bind(x => Result.Ok(x * 2));
        Assert.True(bound.IsSuccess);
        Assert.Equal(10, bound.Value);
    }

    [Fact]
    public void Bind_OnFailure_ForwardsErrors()
    {
        var result = Result.Fail<int>("original error");
        var bound = result.Bind(x => Result.Ok(x * 2));
        Assert.True(bound.IsFailure);
        Assert.Contains("original error", bound.Errors);
    }

    [Fact]
    public void Bind_WhenBindReturnsFailure_PropagatesFailure()
    {
        var result = Result.Ok(5);
        var bound = result.Bind(_ => Result.Fail<int>("bind failed"));
        Assert.True(bound.IsFailure);
        Assert.Contains("bind failed", bound.Errors);
    }

    // ── Match ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Match_OnSuccess_CallsOnSuccess()
    {
        var result = Result.Ok(42);
        var output = result.Match(v => $"ok:{v}", errors => $"fail:{errors.Count}");
        Assert.Equal("ok:42", output);
    }

    [Fact]
    public void Match_OnFailure_CallsOnFailure()
    {
        var result = Result.Fail<int>(new[] { "err1", "err2" });
        var output = result.Match(v => $"ok:{v}", errors => $"fail:{errors.Count}");
        Assert.Equal("fail:2", output);
    }

    // ── OnSuccess ─────────────────────────────────────────────────────────────

    [Fact]
    public void OnSuccess_OnSuccess_InvokesAction()
    {
        var called = false;
        Result.Ok(1).OnSuccess(_ => called = true);
        Assert.True(called);
    }

    [Fact]
    public void OnSuccess_OnFailure_DoesNotInvokeAction()
    {
        var called = false;
        Result.Fail<int>("e").OnSuccess(_ => called = true);
        Assert.False(called);
    }

    [Fact]
    public void OnSuccess_ReturnsOriginalResult()
    {
        var result = Result.Ok(99);
        var returned = result.OnSuccess(_ => { });
        Assert.Same(result, returned);
    }

    // ── OnFailure ─────────────────────────────────────────────────────────────

    [Fact]
    public void OnFailure_OnFailure_InvokesAction()
    {
        var called = false;
        Result.Fail<int>("e").OnFailure(_ => called = true);
        Assert.True(called);
    }

    [Fact]
    public void OnFailure_OnSuccess_DoesNotInvokeAction()
    {
        var called = false;
        Result.Ok(1).OnFailure(_ => called = true);
        Assert.False(called);
    }

    // ── Recover ───────────────────────────────────────────────────────────────

    [Fact]
    public void Recover_OnFailure_ReturnsFallback()
    {
        var result = Result.Fail<int>("err").Recover(_ => -1);
        Assert.True(result.IsSuccess);
        Assert.Equal(-1, result.Value);
    }

    [Fact]
    public void Recover_OnSuccess_ReturnsOriginalValue()
    {
        var result = Result.Ok(42).Recover(_ => -1);
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    // ── ToApiResponse ─────────────────────────────────────────────────────────

    [Fact]
    public void ToApiResponse_OnSuccess_ReturnsSuccessEnvelope()
    {
        var resp = Result.Ok("hello").ToApiResponse();
        Assert.True(resp.Success);
        Assert.Equal("hello", resp.Data);
        Assert.Empty(resp.Errors);
    }

    [Fact]
    public void ToApiResponse_OnFailure_ReturnsFailureEnvelope()
    {
        var resp = Result.Fail<string>("oops").ToApiResponse();
        Assert.False(resp.Success);
        Assert.Contains("oops", resp.Errors);
    }

    [Fact]
    public void ToApiResponse_WithSuccessMessage_SetsMessage()
    {
        var resp = Result.Ok("data").ToApiResponse("Created!");
        Assert.Equal("Created!", resp.Message);
    }

    [Fact]
    public void ToApiResponse_NullResult_ThrowsArgumentNullException()
    {
        Result<string> result = null!;
        Assert.Throws<ArgumentNullException>(() => result.ToApiResponse());
    }

    // ── ToActionResult (generic) ──────────────────────────────────────────────

    [Fact]
    public void ToActionResult_OnSuccess_ReturnsOkObjectResult()
    {
        var actionResult = Result.Ok(42).ToActionResult();
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        var resp = Assert.IsType<ApiResponse<int>>(ok.Value);
        Assert.True(resp.Success);
        Assert.Equal(42, resp.Data);
    }

    [Fact]
    public void ToActionResult_OnFailure_ReturnsBadRequestObjectResult()
    {
        var actionResult = Result.Fail<int>("bad").ToActionResult();
        var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
        var resp = Assert.IsType<ApiResponse<int>>(bad.Value);
        Assert.False(resp.Success);
        Assert.Contains("bad", resp.Errors);
    }

    // ── ToActionResult (Unit) ─────────────────────────────────────────────────

    [Fact]
    public void ToActionResult_Unit_OnSuccess_ReturnsOkObjectResult()
    {
        var actionResult = Result.Ok().ToActionResult("done");
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        var resp = Assert.IsType<ApiResponse>(ok.Value);
        Assert.True(resp.Success);
        Assert.Equal("done", resp.Message);
    }

    [Fact]
    public void ToActionResult_Unit_OnFailure_ReturnsBadRequestObjectResult()
    {
        var actionResult = Result.Fail("failed").ToActionResult();
        var bad = Assert.IsType<BadRequestObjectResult>(actionResult);
        var resp = Assert.IsType<ApiResponse>(bad.Value);
        Assert.False(resp.Success);
        Assert.Contains("failed", resp.Errors);
    }
}
