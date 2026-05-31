using CommonUtils.Exceptions;
using CommonUtils.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;

namespace CommonUtils.Tests;

public class ValidateModelFilterTests
{
    private static ActionExecutingContext MakeContext(ModelStateDictionary modelState)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor(),
            modelState);

        return new ActionExecutingContext(
            actionContext,
            filters: [],
            actionArguments: new Dictionary<string, object?>(),
            controller: new object());
    }

    [Fact]
    public void OnActionExecuting_ValidModelState_DoesNotThrow()
    {
        var filter = new ValidateModelFilter();
        var context = MakeContext(new ModelStateDictionary()); // valid
        var ex = Record.Exception(() => filter.OnActionExecuting(context));
        Assert.Null(ex);
    }

    [Fact]
    public void OnActionExecuting_InvalidModelState_ThrowsValidationException()
    {
        var ms = new ModelStateDictionary();
        ms.AddModelError("Name", "Required");

        var filter = new ValidateModelFilter();
        var context = MakeContext(ms);

        var ex = Assert.Throws<ValidationException>(() => filter.OnActionExecuting(context));
        Assert.Equal(422, ex.StatusCode);
        Assert.True(ex.Errors.ContainsKey("Name"));
    }

    [Fact]
    public void OnActionExecuting_MultipleErrors_AllMapped()
    {
        var ms = new ModelStateDictionary();
        ms.AddModelError("Name", "Required");
        ms.AddModelError("Email", "Invalid format");

        var filter = new ValidateModelFilter();
        var context = MakeContext(ms);

        var ex = Assert.Throws<ValidationException>(() => filter.OnActionExecuting(context));
        Assert.True(ex.Errors.ContainsKey("Name"));
        Assert.True(ex.Errors.ContainsKey("Email"));
    }

    [Fact]
    public void OnActionExecuting_NullContext_ThrowsArgumentNullException()
    {
        var filter = new ValidateModelFilter();
        Assert.Throws<ArgumentNullException>(() => filter.OnActionExecuting(null!));
    }
}
