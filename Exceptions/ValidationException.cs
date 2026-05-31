#pragma warning disable CA1032 // Standard exception constructors omitted intentionally — status code is required

using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CommonUtils.Exceptions;

/// <summary>
/// HTTP 422 — the request is well-formed but contains semantic validation errors.
/// Carries an optional field-level error dictionary for use with validation libraries
/// (FluentValidation, DataAnnotations, etc.).
/// </summary>
public sealed class ValidationException : ApiException
{
    /// <summary>Field-level errors keyed by property name, each with one or more messages.</summary>
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    /// <summary>Single message with no field-level breakdown.</summary>
    public ValidationException(string message, string? errorCode = null)
        : base(422, message, errorCode)
    {
        Errors = new ReadOnlyDictionary<string, string[]>(new Dictionary<string, string[]>());
    }

    /// <summary>Full field-level error dictionary (e.g. from FluentValidation results).</summary>
    public ValidationException(IDictionary<string, string[]> errors)
        : base(422, "One or more validation errors occurred.")
    {
        Errors = new ReadOnlyDictionary<string, string[]>(errors);
    }

    /// <summary>Single field with one or more messages.</summary>
    public ValidationException(string field, params string[] messages)
        : base(422, "One or more validation errors occurred.")
    {
        Errors = new ReadOnlyDictionary<string, string[]>(
            new Dictionary<string, string[]> { [field] = messages });
    }

    /// <summary>
    /// Creates a <see cref="ValidationException"/> from an ASP.NET Core
    /// <see cref="ModelStateDictionary"/>, mapping each invalid field to its error messages.
    /// </summary>
    public static ValidationException FromModelState(ModelStateDictionary modelState)
    {
        ArgumentNullException.ThrowIfNull(modelState);

        var errors = modelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

        return new ValidationException(errors);
    }
}

