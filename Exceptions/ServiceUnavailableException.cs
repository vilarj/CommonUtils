#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 503 — the service is temporarily unavailable (maintenance, overload, downstream failure).</summary>
public sealed class ServiceUnavailableException : ApiException
{
    public ServiceUnavailableException(string message = "Service temporarily unavailable.", string? errorCode = null)
        : base(503, message, errorCode) { }
}
