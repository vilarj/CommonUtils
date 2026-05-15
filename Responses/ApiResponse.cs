namespace CommonUtils.Responses;

/// <summary>
/// Uniform response envelope for endpoints that return data.
/// Use the factory methods on the non-generic <see cref="ApiResponse"/> class:
/// <c>ApiResponse.Ok(user)</c> / <c>ApiResponse.Fail&lt;User&gt;("not found")</c>
/// </summary>
public record ApiResponse<T>
{
    /// <summary><c>true</c> when the operation succeeded.</summary>
    public bool Success { get; init; }

    /// <summary>The response payload. Null on failure.</summary>
    public T? Data { get; init; }

    /// <summary>Optional human-readable message (e.g. "User created successfully.").</summary>
    public string? Message { get; init; }

    /// <summary>Error messages. Empty on success.</summary>
    public IReadOnlyList<string> Errors { get; init; } = [];
}

/// <summary>
/// Uniform response envelope for endpoints that return no payload
/// (DELETE, fire-and-forget commands, etc.) and static factory for
/// <see cref="ApiResponse{T}"/> so callers never need to specify the type argument.
/// </summary>
public record ApiResponse
{
    /// <summary><c>true</c> when the operation succeeded.</summary>
    public bool Success { get; init; }

    /// <summary>Optional human-readable message.</summary>
    public string? Message { get; init; }

    /// <summary>Error messages. Empty on success.</summary>
    public IReadOnlyList<string> Errors { get; init; } = [];

    // ── Non-generic (no-data) factories ──────────────────────────────────────

    /// <summary>Returns a successful no-data response with an optional message.</summary>
    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };

    /// <summary>Returns a failed no-data response with a single error message.</summary>
    public static ApiResponse Fail(string error) =>
        new() { Success = false, Errors = [error] };

    /// <summary>Returns a failed no-data response with multiple error messages.</summary>
    public static ApiResponse Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = errors.ToList().AsReadOnly() };

    // ── Generic (with-data) factories ─────────────────────────────────────────

    /// <summary>Returns a successful response wrapping <paramref name="data"/>.</summary>
    public static ApiResponse<T> Ok<T>(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    /// <summary>Returns a failed response with a single error message.</summary>
    public static ApiResponse<T> Fail<T>(string error) =>
        new() { Success = false, Errors = [error] };

    /// <summary>Returns a failed response with multiple error messages.</summary>
    public static ApiResponse<T> Fail<T>(IEnumerable<string> errors) =>
        new() { Success = false, Errors = errors.ToList().AsReadOnly() };
}
