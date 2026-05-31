using CommonUtils.Results;

namespace CommonUtils.Tests;

public class ResultTaskExtensionsTests
{
    // ── MapAsync (async map) ──────────────────────────────────────────────────

    [Fact]
    public async Task MapAsync_AsyncMap_OnSuccess_ProjectsValue()
    {
        var task = Task.FromResult(Result.Ok(5));
        var result = await task.MapAsync(async x => { await Task.Yield(); return x * 2; });
        Assert.True(result.IsSuccess);
        Assert.Equal(10, result.Value);
    }

    [Fact]
    public async Task MapAsync_AsyncMap_OnFailure_ForwardsErrors()
    {
        var task = Task.FromResult(Result.Fail<int>("err"));
        var result = await task.MapAsync(async x => { await Task.Yield(); return x * 2; });
        Assert.True(result.IsFailure);
        Assert.Contains("err", result.Errors);
    }

    // ── MapAsync (sync map) ───────────────────────────────────────────────────

    [Fact]
    public async Task MapAsync_SyncMap_OnSuccess_ProjectsValue()
    {
        var task = Task.FromResult(Result.Ok(3));
        var result = await task.MapAsync(x => x.ToString());
        Assert.True(result.IsSuccess);
        Assert.Equal("3", result.Value);
    }

    [Fact]
    public async Task MapAsync_SyncMap_OnFailure_ForwardsErrors()
    {
        var task = Task.FromResult(Result.Fail<int>("e"));
        var result = await task.MapAsync(x => x.ToString());
        Assert.True(result.IsFailure);
        Assert.Contains("e", result.Errors);
    }

    // ── BindAsync (async bind) ────────────────────────────────────────────────

    [Fact]
    public async Task BindAsync_AsyncBind_OnSuccess_InvokesBind()
    {
        var task = Task.FromResult(Result.Ok(4));
        var result = await task.BindAsync(async x =>
        {
            await Task.Yield();
            return Result.Ok(x + 1);
        });
        Assert.True(result.IsSuccess);
        Assert.Equal(5, result.Value);
    }

    [Fact]
    public async Task BindAsync_AsyncBind_OnFailure_ForwardsErrors()
    {
        var task = Task.FromResult(Result.Fail<int>("orig"));
        var result = await task.BindAsync(async x =>
        {
            await Task.Yield();
            return Result.Ok(x + 1);
        });
        Assert.True(result.IsFailure);
        Assert.Contains("orig", result.Errors);
    }

    // ── BindAsync (sync bind) ─────────────────────────────────────────────────

    [Fact]
    public async Task BindAsync_SyncBind_OnSuccess_InvokesBind()
    {
        var task = Task.FromResult(Result.Ok(10));
        var result = await task.BindAsync(x => Result.Ok(x - 1));
        Assert.True(result.IsSuccess);
        Assert.Equal(9, result.Value);
    }

    [Fact]
    public async Task BindAsync_SyncBind_WhenBindFails_PropagatesFailure()
    {
        var task = Task.FromResult(Result.Ok(10));
        var result = await task.BindAsync(_ => Result.Fail<int>("bind err"));
        Assert.True(result.IsFailure);
        Assert.Contains("bind err", result.Errors);
    }

    // ── MatchAsync ────────────────────────────────────────────────────────────

    [Fact]
    public async Task MatchAsync_OnSuccess_CallsOnSuccess()
    {
        var task = Task.FromResult(Result.Ok(7));
        var output = await task.MatchAsync(
            async v => { await Task.Yield(); return $"ok:{v}"; },
            async e => { await Task.Yield(); return $"fail:{e.Count}"; });
        Assert.Equal("ok:7", output);
    }

    [Fact]
    public async Task MatchAsync_OnFailure_CallsOnFailure()
    {
        var task = Task.FromResult(Result.Fail<int>(new[] { "x", "y" }));
        var output = await task.MatchAsync(
            async v => { await Task.Yield(); return $"ok:{v}"; },
            async e => { await Task.Yield(); return $"fail:{e.Count}"; });
        Assert.Equal("fail:2", output);
    }

    // ── Null guards ───────────────────────────────────────────────────────────

    [Fact]
    public async Task MapAsync_NullTask_ThrowsArgumentNullException()
    {
        Task<Result<int>> nullTask = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(() => nullTask.MapAsync(x => x * 2));
    }

    [Fact]
    public async Task BindAsync_NullTask_ThrowsArgumentNullException()
    {
        Task<Result<int>> nullTask = null!;
        await Assert.ThrowsAsync<ArgumentNullException>(() => nullTask.BindAsync(x => Result.Ok(x)));
    }
}
