# API Utilities

A lightweight .NET library of reusable building blocks for API development: input guards, string helpers, uniform response envelopes, pagination, and HTTP-aware exceptions.

[![NuGet](https://img.shields.io/nuget/v/API-Utilities)](https://www.nuget.org/packages/API-Utilities)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.txt)

---

## Installation

```bash
dotnet add package API-Utilities
```

**Target framework:** .NET 10.0

---

## Namespaces

| Namespace | Contents |
|---|---|
| `CommonUtils.Checks` | `Check` — static guard methods |
| `CommonUtils.Extensions` | `StringExtensions` — string helpers |
| `CommonUtils.Responses` | `ApiResponse`, `ApiResponse<T>` — response envelope |
| `CommonUtils.Pagination` | `PaginationParams`, `PagedResult<T>` |
| `CommonUtils.Exceptions` | `ApiException` and HTTP-specific subclasses |

---

## Check — Input Guards

`Check` is a static guard class. Every method validates a value and **returns it unchanged** if valid, so guards compose naturally inline or at the top of a method body. On failure it throws a standard .NET argument exception — your middleware decides how to map that to an HTTP response.

```csharp
using CommonUtils.Checks;

public void CreateOrder(string customerId, int quantity, decimal price)
{
    customerId = Check.NotEmpty(customerId, nameof(customerId)); // trims and returns
    quantity   = Check.Positive(quantity, nameof(quantity));
    price      = Check.Positive(price, nameof(price));
}
```

### String guards

| Method | Throws | Notes |
|---|---|---|
| `NotEmpty(string?, paramName)` | `ArgumentException` | Null, empty, or whitespace. Returns trimmed value. |
| `MaxLength(string, max, paramName)` | `ArgumentException` | Trims before measuring. Returns original value. |
| `MinLength(string, min, paramName)` | `ArgumentException` | Trims before measuring. Returns original value. |
| `Length(string, min, max, paramName)` | `ArgumentException` | Combined min + max check. |

### Numeric guards — `int`, `long`, `decimal`, `double`

| Method | Throws | Notes |
|---|---|---|
| `Positive(value, paramName)` | `ArgumentOutOfRangeException` | value > 0 |
| `NotNegative(value, paramName)` | `ArgumentOutOfRangeException` | value ≥ 0 |
| `InRange(value, min, max, paramName)` | `ArgumentOutOfRangeException` | `int` and `decimal` overloads |

### Collection guards

| Method | Throws | Notes |
|---|---|---|
| `NotNull<T>(value, paramName)` | `ArgumentNullException` | Any reference type |
| `NotEmpty<T>(IEnumerable<T>?, paramName)` | `ArgumentException` | Null or empty |
| `MaxCount<T>(ICollection<T>, max, paramName)` | `ArgumentException` | — |
| `MinCount<T>(ICollection<T>, min, paramName)` | `ArgumentException` | — |

### Other guards

| Method | Throws | Notes |
|---|---|---|
| `NotEmpty(Guid, paramName)` | `ArgumentException` | `Guid.Empty` check |
| `Defined<T>(T, paramName)` | `ArgumentException` | Enum value must be declared |
| `NotDefault(DateTime, paramName)` | `ArgumentException` | Rejects `DateTime.MinValue` |
| `NotDefault(DateTimeOffset, paramName)` | `ArgumentException` | — |
| `NotInPast(DateTime, paramName)` | `ArgumentOutOfRangeException` | Calls `NotDefault` first |
| `NotInFuture(DateTime, paramName)` | `ArgumentOutOfRangeException` | Calls `NotDefault` first |
| `NotInPast(DateTimeOffset, paramName)` | `ArgumentOutOfRangeException` | — |
| `NotInFuture(DateTimeOffset, paramName)` | `ArgumentOutOfRangeException` | — |

```csharp
var id      = Check.NotEmpty(dto.Id, nameof(dto.Id));
var tags    = Check.NotEmpty(dto.Tags, nameof(dto.Tags));
var role    = Check.Defined(dto.Role, nameof(dto.Role));
var expires = Check.NotInPast(dto.ExpiresAt, nameof(dto.ExpiresAt));
```

---

## StringExtensions — String Helpers

```csharp
using CommonUtils.Extensions;
```

### `Normalize`

Trims leading/trailing whitespace and collapses internal runs of any whitespace (`\t`, `\n`, multiple spaces) to a single space. Useful for sanitising name and address fields from form input.

> **Note:** Because `string` has a built-in `Normalize()` instance method (Unicode normalization), calling `.Normalize()` on a string will resolve to the BCL method, not this one. Call it explicitly as a static method:

```csharp
string clean = StringExtensions.Normalize("  hello\t world  "); // "hello world"
```

### `Truncate`

Returns the string as-is if it fits, or cuts it to exactly `maxLength` characters — never throws.

```csharp
string preview = description.Truncate(160);
```

### `NullIfEmpty`

Returns `null` for `null`, empty, or whitespace-only strings. Useful for optional fields stored as `NULL` in the database rather than empty strings.

```csharp
string? nickname = dto.Nickname.NullIfEmpty(); // null when blank
```

### `ToSnakeCase`

Converts PascalCase or camelCase to `snake_case`. Breaks on lowercase/digit → uppercase transitions.

```csharp
"OrderId".ToSnakeCase()       // "order_id"
"createdAt".ToSnakeCase()     // "created_at"
"Version2Value".ToSnakeCase() // "version2_value"
```

> **Acronym behaviour:** Only the last uppercase letter in a run is treated as a word boundary. `"OrderID"` → `"order_id"` (breaks before `D`). `"HTTPServer"` → `"httpserver"` (no internal break).

---

## ApiResponse — Uniform Response Envelope

All endpoints return the same shape, making client-side parsing predictable.

```csharp
using CommonUtils.Responses;
```

### Endpoints that return data

```csharp
// success
return Ok(ApiResponse.Ok(user));

// failure
return BadRequest(ApiResponse.Fail<User>("User not found."));

// multiple errors
return UnprocessableEntity(ApiResponse.Fail<User>(errors));
```

**Shape:**

```json
{ "success": true,  "data": { ... }, "message": "optional", "errors": [] }
{ "success": false, "data": null,    "message": null,        "errors": ["..."] }
```

### Endpoints with no payload (DELETE, commands, etc.)

```csharp
return Ok(ApiResponse.Ok("Order cancelled."));

return BadRequest(ApiResponse.Fail("Insufficient stock."));
```

**Shape:**

```json
{ "success": true,  "message": "Order cancelled.", "errors": [] }
{ "success": false, "message": null,               "errors": ["Insufficient stock."] }
```

---

## Pagination

### `PaginationParams`

Bind directly from the query string. Defaults to page 1, page size 20.

```csharp
using CommonUtils.Pagination;

// GET /orders?page=2&pageSize=50
[HttpGet]
public IActionResult GetOrders([FromQuery] PaginationParams pagination)
{
    pagination.Validate(); // throws BadRequestException on bad input

    var items = _repo.GetOrders(skip: pagination.Skip, take: pagination.Take);
    // ...
}
```

`Validate(maxPageSize)` throws `BadRequestException` if `Page < 1`, `PageSize < 1`, or `PageSize > maxPageSize` (default 100).

### `PagedResult<T>`

```csharp
var items      = await _db.Orders.Skip(p.Skip).Take(p.Take).ToListAsync();
var totalCount = await _db.Orders.CountAsync();

PagedResult<OrderDto> result = PagedResult.Create(items, totalCount, pagination);

return Ok(ApiResponse.Ok(result));
```

**Properties on `PagedResult<T>`:**

| Property | Type | Description |
|---|---|---|
| `Items` | `IReadOnlyList<T>` | Current page items |
| `Page` | `int` | Current page (1-based) |
| `PageSize` | `int` | Items per page |
| `TotalCount` | `int` | Total items across all pages |
| `TotalPages` | `int` | Computed: `⌈TotalCount / PageSize⌉` |
| `HasNextPage` | `bool` | `Page < TotalPages` |
| `HasPreviousPage` | `bool` | `Page > 1` |

Use `PagedResult.Empty<T>(pagination)` when the data source returns nothing.

---

## Exceptions

All exceptions extend `ApiException`, which carries an HTTP `StatusCode` and an optional machine-readable `ErrorCode`. Use a single exception-handling middleware to map them to responses — no per-endpoint try/catch needed.

```csharp
using CommonUtils.Exceptions;
```

### Available exceptions

| Class | Status | When to use |
|---|---|---|
| `BadRequestException` | 400 | Malformed input, failed preconditions |
| `UnauthorizedException` | 401 | Missing or invalid credentials |
| `ForbiddenException` | 403 | Authenticated but not permitted |
| `NotFoundException` | 404 | Resource does not exist |
| `ConflictException` | 409 | Duplicate or state conflict |
| `ValidationException` | 422 | Semantic validation errors (field-level) |

### Usage

```csharp
// Simple message
throw new NotFoundException("Order not found.", errorCode: "ORDER_NOT_FOUND");

// Convenience constructor (non-string key)
throw new NotFoundException("Order", orderId); // "Order with key '42' was not found."

// Field-level validation errors
throw new ValidationException("Email", "must be a valid email address");

// Multiple fields (e.g. from FluentValidation)
throw new ValidationException(new Dictionary<string, string[]>
{
    ["Email"]    = ["Required", "Must be a valid email"],
    ["Password"] = ["Must be at least 8 characters"],
});
```

### Middleware integration (ASP.NET Core)

```csharp
app.UseExceptionHandler(builder => builder.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    if (ex is ApiException apiEx)
    {
        context.Response.StatusCode = apiEx.StatusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            success = false,
            errorCode = apiEx.ErrorCode,
            errors = ex is ValidationException vex && vex.Errors.Count > 0
                ? vex.Errors
                : new[] { ex.Message }
        });
        return;
    }

    context.Response.StatusCode = 500;
    await context.Response.WriteAsJsonAsync(new { success = false, errors = new[] { "An unexpected error occurred." } });
}));
```

---

## License

MIT — see [LICENSE.txt](LICENSE.txt).
