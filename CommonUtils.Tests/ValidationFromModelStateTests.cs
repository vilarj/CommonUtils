using CommonUtils.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CommonUtils.Tests;

public class ValidationFromModelStateTests
{
    private static ModelStateDictionary MakeModelState(params (string Field, string Message)[] errors)
    {
        var ms = new ModelStateDictionary();
        foreach (var (field, msg) in errors)
            ms.AddModelError(field, msg);
        return ms;
    }

    [Fact]
    public void FromModelState_WithErrors_ThrowableAsValidationException()
    {
        var ms = MakeModelState(("Name", "Required"), ("Email", "Invalid"));
        var ex = ValidationException.FromModelState(ms);
        Assert.IsType<ValidationException>(ex);
        Assert.Equal(422, ex.StatusCode);
        Assert.True(ex.Errors.ContainsKey("Name"));
        Assert.True(ex.Errors.ContainsKey("Email"));
    }

    [Fact]
    public void FromModelState_IncludesAllMessagesPerField()
    {
        var ms = new ModelStateDictionary();
        ms.AddModelError("Age", "Must be positive");
        ms.AddModelError("Age", "Must be under 150");
        var ex = ValidationException.FromModelState(ms);
        Assert.Equal(2, ex.Errors["Age"].Length);
    }

    [Fact]
    public void FromModelState_EmptyModelState_ReturnsExceptionWithNoErrors()
    {
        var ex = ValidationException.FromModelState(new ModelStateDictionary());
        Assert.Empty(ex.Errors);
    }

    [Fact]
    public void FromModelState_NullModelState_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => ValidationException.FromModelState(null!));
    }
}
