using CommonUtils.Responses;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Results;

/// <summary>
/// Chaining and transformation extensions for <see cref="Result{T}"/>.
/// These methods allow building pipelines without manual <c>if (result.IsFailure)</c> checks.
/// </summary>
public static class ResultExtensions
{
    // ── Transformation ────────────────────────────────────────────────────────

    /// <summary>
    /// Projects the value of a successful result. If the result is a failure the errors
    /// are forwarded unchanged and <paramref name="map"/> is not called.
    /// </summary>
    public static Result<TOut> Map<T, TOut>(this Result<T> result, Func<T, TOut> map)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(map);
        return result.IsFailure
            ? Result.Fail<TOut>(result.Errors)
            : Result.Ok(map(result.Value!));
    }

    /// <summary>
    /// Chains a result-returning function. If the current result is a failure the errors
    /// are forwarded and <paramref name="bind"/> is not called.
    /// </summary>
    public static Result<TOut> Bind<T, TOut>(this Result<T> result, Func<T, Result<TOut>> bind)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(bind);
        return result.IsFailure
            ? Result.Fail<TOut>(result.Errors)
            : bind(result.Value!);
    }

    /// <summary>
    /// Collapses both success and failure paths into a single value.
    /// </summary>
    public static TOut Match<T, TOut>(this Result<T> result, Func<T, TOut> onSuccess, Func<IReadOnlyList<string>, TOut> onFailure)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);
        return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.Errors);
    }

    // ── Side-effects ──────────────────────────────────────────────────────────

    /// <summary>
    /// Invokes <paramref name="action"/> with the value when the result is successful.
    /// Returns the original result unchanged so the chain can continue.
    /// </summary>
    public static Result<T> OnSuccess<T>(this Result<T> result, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(action);
        if (result.IsSuccess) action(result.Value!);
        return result;
    }

    /// <summary>
    /// Invokes <paramref name="action"/> with the errors when the result is a failure.
    /// Returns the original result unchanged so the chain can continue.
    /// </summary>
    public static Result<T> OnFailure<T>(this Result<T> result, Action<IReadOnlyList<string>> action)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(action);
        if (result.IsFailure) action(result.Errors);
        return result;
    }

    // ── Recovery ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Attempts to recover from a failure by supplying a fallback value.
    /// On success the fallback is ignored.
    /// </summary>
    public static Result<T> Recover<T>(this Result<T> result, Func<IReadOnlyList<string>, T> fallback)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(fallback);
        return result.IsFailure ? Result.Ok(fallback(result.Errors)) : result;
    }

    // ── HTTP adapters ─────────────────────────────────────────────────────────

    /// <summary>
    /// Converts the result to a <see cref="ApiResponse{T}"/> — success maps to
    /// <see cref="ApiResponse.Ok{T}(T, string?)"/>, failure maps to
    /// <see cref="ApiResponse.Fail{T}(System.Collections.Generic.IEnumerable{string})"/>.
    /// </summary>
    public static ApiResponse<T> ToApiResponse<T>(this Result<T> result, string? successMessage = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess
            ? ApiResponse.Ok(result.Value!, successMessage)
            : ApiResponse.Fail<T>(result.Errors);
    }

    /// <summary>
    /// Converts the result to an <see cref="IActionResult"/>.
    /// Success returns <see cref="OkObjectResult"/> wrapping an <see cref="ApiResponse{T}"/>.
    /// Failure returns <see cref="BadRequestObjectResult"/> wrapping an <see cref="ApiResponse{T}"/>.
    /// </summary>
    public static IActionResult ToActionResult<T>(this Result<T> result, string? successMessage = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess
            ? new OkObjectResult(ApiResponse.Ok(result.Value!, successMessage))
            : new BadRequestObjectResult(ApiResponse.Fail<T>(result.Errors));
    }

    /// <summary>
    /// Converts a no-value <c>Result&lt;Unit&gt;</c> to an <see cref="IActionResult"/>.
    /// Success returns <see cref="OkObjectResult"/> wrapping an <see cref="ApiResponse"/>.
    /// Failure returns <see cref="BadRequestObjectResult"/> wrapping an <see cref="ApiResponse"/>.
    /// </summary>
    public static IActionResult ToActionResult(this Result<Unit> result, string? successMessage = null)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccess
            ? new OkObjectResult(ApiResponse.Ok(successMessage))
            : new BadRequestObjectResult(ApiResponse.Fail(result.Errors));
    }
}
