namespace CommonUtils.Results;

/// <summary>
/// Async-friendly extensions for <c>Task&lt;Result&lt;T&gt;&gt;</c> pipelines.
/// Allows chaining async operations without intermediate <c>await</c> + <c>if</c> blocks.
/// </summary>
public static class ResultTaskExtensions
{
    // ── Async Transformation ──────────────────────────────────────────────────

    /// <summary>
    /// Asynchronously projects the value of a successful result.
    /// If the result is a failure the errors are forwarded and <paramref name="map"/> is not called.
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TOut>> map)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(map);
        var result = await resultTask.ConfigureAwait(false);
        return result.IsFailure
            ? Result.Fail<TOut>(result.Errors)
            : Result.Ok(await map(result.Value!).ConfigureAwait(false));
    }

    /// <summary>
    /// Overload for synchronous projections on an async result.
    /// </summary>
    public static async Task<Result<TOut>> MapAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(map);
        var result = await resultTask.ConfigureAwait(false);
        return result.IsFailure
            ? Result.Fail<TOut>(result.Errors)
            : Result.Ok(map(result.Value!));
    }

    // ── Async Bind ────────────────────────────────────────────────────────────

    /// <summary>
    /// Chains an async result-returning function. If the current result is a failure
    /// the errors are forwarded and <paramref name="bind"/> is not called.
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TOut>>> bind)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(bind);
        var result = await resultTask.ConfigureAwait(false);
        return result.IsFailure
            ? Result.Fail<TOut>(result.Errors)
            : await bind(result.Value!).ConfigureAwait(false);
    }

    /// <summary>
    /// Overload for synchronous result-returning functions on an async result.
    /// </summary>
    public static async Task<Result<TOut>> BindAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(bind);
        var result = await resultTask.ConfigureAwait(false);
        return result.IsFailure
            ? Result.Fail<TOut>(result.Errors)
            : bind(result.Value!);
    }

    // ── Async Match ───────────────────────────────────────────────────────────

    /// <summary>
    /// Asynchronously collapses both success and failure paths into a single value.
    /// </summary>
    public static async Task<TOut> MatchAsync<T, TOut>(
        this Task<Result<T>> resultTask,
        Func<T, Task<TOut>> onSuccess,
        Func<IReadOnlyList<string>, Task<TOut>> onFailure)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        var result = await resultTask.ConfigureAwait(false);
        return result.IsSuccess
            ? await onSuccess(result.Value!).ConfigureAwait(false)
            : await onFailure(result.Errors).ConfigureAwait(false);
    }
}
