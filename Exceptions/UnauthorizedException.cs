#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 401 — the caller is not authenticated.</summary>
public sealed class UnauthorizedException(string message = "Unauthorized.", string? errorCode = null)
    : ApiException(401, message, errorCode);
