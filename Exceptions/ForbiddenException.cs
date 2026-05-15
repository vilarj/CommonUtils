#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 403 — the caller is authenticated but lacks permission.</summary>
public sealed class ForbiddenException : ApiException
{
    public ForbiddenException(string message = "Access denied.", string? errorCode = null)
        : base(403, message, errorCode) { }
}
