using CommonUtils.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommonUtils.Middleware;

/// <summary>
/// Options that control how <see cref="ExceptionHandlingExtensions.UseCommonApiExceptionHandling"/>
/// maps exceptions to responses.
/// </summary>
public sealed class ExceptionHandlingOptions
{
    /// <summary>
    /// When <c>true</c>, unhandled exceptions include an internal error detail
    /// in the response. Keep <c>false</c> in production.
    /// </summary>
    public bool IncludeExceptionDetails { get; set; }

    /// <summary>
    /// When <c>true</c>, responses use RFC 7807 <see cref="ProblemDetails"/> format
    /// instead of the default <c>{ success, errors }</c> envelope.
    /// </summary>
    public bool UseProblemDetails { get; set; }
}

/// <summary>
/// ASP.NET Core exception handling middleware that maps <see cref="ApiException"/>
/// subclasses to uniform JSON responses, eliminating the need for per-project
/// <c>UseExceptionHandler</c> boilerplate.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CommonApiExceptionHandler(ExceptionHandlingOptions options) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ApiException apiEx)
        {
            httpContext.Response.StatusCode = apiEx.StatusCode;

            if (apiEx is TooManyRequestsException tooMany && tooMany.RetryAfter.HasValue)
                httpContext.Response.Headers["Retry-After"] = ((int)tooMany.RetryAfter.Value.TotalSeconds).ToString(System.Globalization.CultureInfo.InvariantCulture);

            if (options.UseProblemDetails)
                await WriteProblemDetailsAsync(httpContext, apiEx, cancellationToken).ConfigureAwait(false);
            else
                await WriteEnvelopeAsync(httpContext, apiEx, cancellationToken).ConfigureAwait(false);

            return true;
        }

        if (!options.IncludeExceptionDetails) return false;
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            new { success = false, errors = new[] { exception.Message } },
            cancellationToken).ConfigureAwait(false);
        return true;

    }

    private static Task WriteEnvelopeAsync(HttpContext context, ApiException ex, CancellationToken ct)
    {
        var errors = ex is ValidationException vex && vex.Errors.Count > 0
            ? vex.Errors.SelectMany(kvp => kvp.Value).ToArray()
            : [ex.Message];

        return context.Response.WriteAsJsonAsync(
            new
            {
                success = false,
                errorCode = ex.ErrorCode,
                errors
            },
            ct);
    }

    private static Task WriteProblemDetailsAsync(HttpContext context, ApiException ex, CancellationToken ct)
    {
        context.Response.ContentType = "application/problem+json";

        var extensions = new Dictionary<string, object?>();

        if (ex.ErrorCode is not null)
            extensions["errorCode"] = ex.ErrorCode;

        if (ex is ValidationException vex && vex.Errors.Count > 0)
            extensions["errors"] = vex.Errors;

        var problem = new ProblemDetails
        {
            Status = ex.StatusCode,
            Title = GetTitle(ex),
            Detail = ex.Message,
            Extensions = extensions
        };

        return context.Response.WriteAsJsonAsync(problem, ct);
    }

    private static string GetTitle(ApiException ex) => ex.StatusCode switch
    {
        400 => "Bad Request",
        401 => "Unauthorized",
        403 => "Forbidden",
        404 => "Not Found",
        409 => "Conflict",
        410 => "Gone",
        422 => "Unprocessable Entity",
        429 => "Too Many Requests",
        503 => "Service Unavailable",
        _ => "An error occurred"
    };
}
