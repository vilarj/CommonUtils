using Microsoft.AspNetCore.Http;

namespace CommonUtils.Middleware;

/// <summary>
/// Provides access to the correlation ID for the current HTTP request.
/// </summary>
public interface ICorrelationIdAccessor
{
    /// <summary>The correlation ID for the current request.</summary>
    string CorrelationId { get; }
}

/// <summary>
/// Reads or generates a correlation ID from/for each incoming request and exposes
/// it via <see cref="ICorrelationIdAccessor"/>. The ID is written back to the response
/// under the same header so clients can correlate requests with logs.
/// </summary>
public sealed class CorrelationIdMiddleware : IMiddleware
{
    /// <summary>Default header name used to carry the correlation ID.</summary>
    public const string DefaultHeaderName = "X-Correlation-ID";

    private readonly CorrelationIdOptions _options;
    private readonly ICorrelationIdAccessor _accessor;

    public CorrelationIdMiddleware(CorrelationIdOptions options, ICorrelationIdAccessor accessor)
    {
        _options = options;
        _accessor = accessor;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(next);
        var id = context.Request.Headers.TryGetValue(_options.HeaderName, out var existing) && !string.IsNullOrWhiteSpace(existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString("N");

        ((CorrelationIdAccessor)_accessor).SetId(id);

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[_options.HeaderName] = id;
            return Task.CompletedTask;
        });

        await next(context).ConfigureAwait(false);
    }
}

/// <summary>
/// Options for <see cref="CorrelationIdMiddleware"/>.
/// </summary>
public sealed class CorrelationIdOptions
{
    /// <summary>Header name to read from and write to. Defaults to <c>X-Correlation-ID</c>.</summary>
    public string HeaderName { get; set; } = CorrelationIdMiddleware.DefaultHeaderName;
}

/// <summary>
/// Default scoped implementation of <see cref="ICorrelationIdAccessor"/>.
/// The middleware sets the ID before the rest of the pipeline runs.
/// </summary>
/// <inheritdoc />
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private string _id = string.Empty;

    public string CorrelationId => _id;

    internal void SetId(string id) => _id = id;
}
