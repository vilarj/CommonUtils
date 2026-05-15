#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>
/// Base class for all API-level exceptions. Carries an HTTP status code so that
/// exception-handling middleware can map it to the correct HTTP response without
/// per-endpoint try/catch blocks.
/// </summary>
public abstract class ApiException : Exception
{
    /// <summary>HTTP status code that should be returned to the client.</summary>
    public int StatusCode { get; }

    /// <summary>
    /// Optional machine-readable error code for client-side branching
    /// (e.g. "USER_NOT_FOUND", "INSUFFICIENT_STOCK").
    /// </summary>
    public string? ErrorCode { get; }

    protected ApiException(int statusCode, string message, string? errorCode = null, Exception? inner = null)
        : base(message, inner)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
    }
}
