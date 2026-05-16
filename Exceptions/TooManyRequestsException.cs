#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 429 — the caller has exceeded the allowed request rate.</summary>
public sealed class TooManyRequestsException : ApiException
{
    /// <summary>
    /// How long the caller should wait before retrying.
    /// Map to the <c>Retry-After</c> response header in middleware.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    public TooManyRequestsException(
        string message = "Too many requests.",
        string? errorCode = null,
        TimeSpan? retryAfter = null)
        : base(429, message, errorCode)
    {
        RetryAfter = retryAfter;
    }
}
