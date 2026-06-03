#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 410 — the resource existed but has been permanently removed.</summary>
public sealed class GoneException(
    string message = "The requested resource is no longer available.",
    string? errorCode = null)
    : ApiException(410, message, errorCode);
