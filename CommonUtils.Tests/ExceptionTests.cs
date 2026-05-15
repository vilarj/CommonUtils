using CommonUtils.Exceptions;

namespace CommonUtils.Tests;

public class BadRequestExceptionTests
{
    [Fact]
    public void StatusCode_IsHttpBadRequest()
    {
        Assert.Equal(400, new BadRequestException("bad").StatusCode);
    }

    [Fact]
    public void Message_IsSet()
    {
        Assert.Equal("invalid input", new BadRequestException("invalid input").Message);
    }

    [Fact]
    public void ErrorCode_IsSetWhenProvided()
    {
        Assert.Equal("INVALID_INPUT", new BadRequestException("bad", "INVALID_INPUT").ErrorCode);
    }

    [Fact]
    public void ErrorCode_IsNullWhenOmitted()
    {
        Assert.Null(new BadRequestException("bad").ErrorCode);
    }

    [Fact]
    public void IsAssignableFrom_ApiException()
    {
        Assert.IsAssignableFrom<ApiException>(new BadRequestException("bad"));
    }
}

public class ValidationExceptionTests
{
    [Fact]
    public void StatusCode_IsHttpUnprocessableEntity()
    {
        Assert.Equal(422, new ValidationException("invalid").StatusCode);
    }

    [Fact]
    public void SingleMessageCtor_SetsMessage()
    {
        Assert.Equal("invalid", new ValidationException("invalid").Message);
    }

    [Fact]
    public void SingleMessageCtor_ErrorsIsEmpty()
    {
        Assert.Empty(new ValidationException("invalid").Errors);
    }

    [Fact]
    public void SingleMessageCtor_SetsErrorCode()
    {
        Assert.Equal("VALIDATION_FAILED", new ValidationException("invalid", "VALIDATION_FAILED").ErrorCode);
    }

    [Fact]
    public void DictionaryCtor_SetsDefaultMessage()
    {
        var ex = new ValidationException(new Dictionary<string, string[]>());
        Assert.Equal("One or more validation errors occurred.", ex.Message);
    }

    [Fact]
    public void DictionaryCtor_ExposesFieldErrors()
    {
        var dict = new Dictionary<string, string[]>
        {
            ["Name"] = ["Name is required."],
            ["Email"] = ["Email is invalid."]
        };
        var ex = new ValidationException(dict);
        Assert.Equal(2, ex.Errors.Count);
        Assert.Equal(["Name is required."], ex.Errors["Name"]);
        Assert.Equal(["Email is invalid."], ex.Errors["Email"]);
    }

    [Fact]
    public void FieldParamsCtor_SetsDefaultMessage()
    {
        var ex = new ValidationException("Email", "Required.", "Must be valid.");
        Assert.Equal("One or more validation errors occurred.", ex.Message);
    }

    [Fact]
    public void FieldParamsCtor_ExposesAllMessagesUnderField()
    {
        var ex = new ValidationException("Email", "Required.", "Must be valid.");
        Assert.Single(ex.Errors);
        Assert.Equal(2, ex.Errors["Email"].Length);
        Assert.Contains("Required.", ex.Errors["Email"]);
        Assert.Contains("Must be valid.", ex.Errors["Email"]);
    }

    [Fact]
    public void IsAssignableFrom_ApiException()
    {
        Assert.IsAssignableFrom<ApiException>(new ValidationException("x"));
    }
}

public class NotFoundExceptionTests
{
    [Fact]
    public void StatusCode_IsHttpNotFound()
    {
        Assert.Equal(404, new NotFoundException("not found").StatusCode);
    }

    [Fact]
    public void Message_IsSet()
    {
        Assert.Equal("Resource not found", new NotFoundException("Resource not found").Message);
    }

    [Fact]
    public void ErrorCode_IsSetWhenProvided()
    {
        Assert.Equal("ITEM_NOT_FOUND", new NotFoundException("not found", "ITEM_NOT_FOUND").ErrorCode);
    }

    [Fact]
    public void ErrorCode_IsNullWhenOmitted()
    {
        Assert.Null(new NotFoundException("not found").ErrorCode);
    }

    [Fact]
    public void ConvenienceCtor_FormatsMessageWithResourceNameAndKey()
    {
        var ex = new NotFoundException("User", 42);
        Assert.Equal("User with key '42' was not found.", ex.Message);
    }

    [Fact]
    public void ConvenienceCtor_WorksWithStringKey()
    {
        // Cast to object to select (resourceName, object key) overload;
        // without it, (message, string? errorCode) is preferred by overload resolution.
        var ex = new NotFoundException("Order", (object)"ORD-123");
        Assert.Equal("Order with key 'ORD-123' was not found.", ex.Message);
    }

    [Fact]
    public void IsAssignableFrom_ApiException()
    {
        Assert.IsAssignableFrom<ApiException>(new NotFoundException("x"));
    }
}

public class ConflictExceptionTests
{
    [Fact]
    public void StatusCode_IsHttpConflict()
    {
        Assert.Equal(409, new ConflictException("conflict").StatusCode);
    }

    [Fact]
    public void Message_IsSet()
    {
        Assert.Equal("Duplicate entry", new ConflictException("Duplicate entry").Message);
    }

    [Fact]
    public void ErrorCode_IsSetWhenProvided()
    {
        Assert.Equal("DUPLICATE", new ConflictException("conflict", "DUPLICATE").ErrorCode);
    }

    [Fact]
    public void ErrorCode_IsNullWhenOmitted()
    {
        Assert.Null(new ConflictException("conflict").ErrorCode);
    }

    [Fact]
    public void IsAssignableFrom_ApiException()
    {
        Assert.IsAssignableFrom<ApiException>(new ConflictException("x"));
    }
}

public class UnauthorizedExceptionTests
{
    [Fact]
    public void StatusCode_IsHttpUnauthorized()
    {
        Assert.Equal(401, new UnauthorizedException().StatusCode);
    }

    [Fact]
    public void DefaultMessage_IsUnauthorized()
    {
        Assert.Equal("Unauthorized.", new UnauthorizedException().Message);
    }

    [Fact]
    public void CustomMessage_IsSet()
    {
        Assert.Equal("Token expired.", new UnauthorizedException("Token expired.").Message);
    }

    [Fact]
    public void ErrorCode_IsSetWhenProvided()
    {
        Assert.Equal("TOKEN_EXPIRED", new UnauthorizedException("Unauthorized.", "TOKEN_EXPIRED").ErrorCode);
    }

    [Fact]
    public void ErrorCode_IsNullWhenOmitted()
    {
        Assert.Null(new UnauthorizedException().ErrorCode);
    }

    [Fact]
    public void IsAssignableFrom_ApiException()
    {
        Assert.IsAssignableFrom<ApiException>(new UnauthorizedException());
    }
}

public class ForbiddenExceptionTests
{
    [Fact]
    public void StatusCode_IsHttpForbidden()
    {
        Assert.Equal(403, new ForbiddenException().StatusCode);
    }

    [Fact]
    public void DefaultMessage_IsAccessDenied()
    {
        Assert.Equal("Access denied.", new ForbiddenException().Message);
    }

    [Fact]
    public void CustomMessage_IsSet()
    {
        Assert.Equal("Insufficient permissions.", new ForbiddenException("Insufficient permissions.").Message);
    }

    [Fact]
    public void ErrorCode_IsSetWhenProvided()
    {
        Assert.Equal("NO_PERMISSION", new ForbiddenException("Access denied.", "NO_PERMISSION").ErrorCode);
    }

    [Fact]
    public void ErrorCode_IsNullWhenOmitted()
    {
        Assert.Null(new ForbiddenException().ErrorCode);
    }

    [Fact]
    public void IsAssignableFrom_ApiException()
    {
        Assert.IsAssignableFrom<ApiException>(new ForbiddenException());
    }
}
