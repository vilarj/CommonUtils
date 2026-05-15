#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

namespace CommonUtils.Exceptions;

/// <summary>HTTP 404 — the requested resource does not exist.</summary>
public sealed class NotFoundException : ApiException
{
    public NotFoundException(string message, string? errorCode = null)
        : base(404, message, errorCode) { }

    /// <summary>Convenience constructor: "{resourceName} with key '{key}' was not found."</summary>
    public NotFoundException(string resourceName, object key)
        : base(404, $"{resourceName} with key '{key}' was not found.") { }
}
