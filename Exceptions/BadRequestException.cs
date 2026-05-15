#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 400 — the request is malformed or contains invalid input.</summary>
public sealed class BadRequestException : ApiException
{
    public BadRequestException(string message, string? errorCode = null)
        : base(400, message, errorCode) { }
}
