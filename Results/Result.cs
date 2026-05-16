namespace CommonUtils.Results;

/// <summary>Represents a void result — the unit type for <see cref="Result{T}"/> when no value is needed.</summary>
public readonly record struct Unit
{
#pragma warning disable CA1805 // default is intentional — makes the sentinel explicit
    /// <summary>The single instance of <see cref="Unit"/>.</summary>
    public static readonly Unit Value = default;
#pragma warning restore CA1805
}

/// <summary>
/// Represents the outcome of an operation — success with a value, or failure with one or more error messages.
/// Use the factory methods on the non-generic <see cref="Result"/> class:
/// <c>Result.Ok(order)</c> / <c>Result.Fail&lt;Order&gt;("not found")</c>
/// </summary>
public sealed record Result<T>
{
    /// <summary><c>true</c> when the operation succeeded.</summary>
    public bool IsSuccess { get; private init; }

    /// <summary>The operation result. Only meaningful when <see cref="IsSuccess"/> is <c>true</c>.</summary>
    public T? Value { get; private init; }

    /// <summary>Error messages. Empty on success.</summary>
    public IReadOnlyList<string> Errors { get; private init; } = [];

    /// <summary><c>true</c> when the operation failed.</summary>
    public bool IsFailure => !IsSuccess;

    private Result() { }

    internal static Result<T> Succeed(T value) =>
        new() { IsSuccess = true, Value = value };

    internal static Result<T> Failure(IReadOnlyList<string> errors) =>
        new() { IsSuccess = false, Errors = errors };

#pragma warning disable CA2225 // Named alternate: Result.Ok<T>(value) serves this role
    /// <summary>Implicitly wraps a value in a successful result.</summary>
    public static implicit operator Result<T>(T value) => Succeed(value);
#pragma warning restore CA2225
}

/// <summary>
/// Static factory for <see cref="Result{T}"/> so callers never need to specify the type argument,
/// and for no-value operations via <see cref="Result{T}">Result&lt;Unit&gt;</see>.
/// </summary>
public static class Result
{
    /// <summary>Returns a successful result wrapping <paramref name="value"/>.</summary>
    public static Result<T> Ok<T>(T value) => Result<T>.Succeed(value);

    /// <summary>Returns a successful no-value result.</summary>
    public static Result<Unit> Ok() => Result<Unit>.Succeed(Unit.Value);

    /// <summary>Returns a failed result with a single error message.</summary>
    public static Result<T> Fail<T>(string error) => Result<T>.Failure([error]);

    /// <summary>Returns a failed result with multiple error messages.</summary>
    public static Result<T> Fail<T>(IEnumerable<string> errors) => Result<T>.Failure([.. errors]);

    /// <summary>Returns a failed no-value result with a single error message.</summary>
    public static Result<Unit> Fail(string error) => Result<Unit>.Failure([error]);

    /// <summary>Returns a failed no-value result with multiple error messages.</summary>
    public static Result<Unit> Fail(IEnumerable<string> errors) => Result<Unit>.Failure([.. errors]);
}
