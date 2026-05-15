#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 409 — the request conflicts with the current state of the resource.</summary>
public sealed class ConflictException : ApiException
{
    public ConflictException(string message, string? errorCode = null)
        : base(409, message, errorCode) { }
}
