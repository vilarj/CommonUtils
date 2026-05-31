using CommonUtils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CommonUtils.Filters;

/// <summary>
/// Action filter that automatically throws a <see cref="ValidationException"/> when
/// <see cref="ActionContext.ModelState"/> is invalid, eliminating the need for
/// <c>if (!ModelState.IsValid)</c> checks in every action method.
/// </summary>
/// <remarks>
/// Register globally in <c>Program.cs</c>:
/// <code>
/// builder.Services.AddControllers(options =>
///     options.Filters.Add&lt;ValidateModelFilter&gt;());
/// </code>
/// Or use the <c>[ValidateModel]</c> attribute on individual controllers or actions.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
public sealed class ValidateModelFilter : ActionFilterAttribute
{
    /// <inheritdoc />
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        if (!context.ModelState.IsValid)
            throw ValidationException.FromModelState(context.ModelState);
    }
}
