# API Utilities

A lightweight .NET library of reusable building blocks for API development: input guards, string helpers, uniform response envelopes, pagination, sorting, HTTP-aware exceptions, a functional result type, shared middleware, auth helpers, and IQueryable extensions.

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

| Namespace                | Contents                                                                                              |
| ------------------------ | ----------------------------------------------------------------------------------------------------- |
| `CommonUtils.Checks`     | `Check` — static guard methods                                                                        |
| `CommonUtils.Extensions` | `StringExtensions` — string helpers                                                                   |
| `CommonUtils.Responses`  | `ApiResponse`, `ApiResponse<T>` — response envelope                                                  |
| `CommonUtils.Pagination` | `PaginationParams`, `PagedResult<T>`, `SortParams`, `MultiSortParams`, `CursorPaginationParams<T>`   |
| `CommonUtils.Exceptions` | `ApiException` and HTTP-specific subclasses                                                           |
| `CommonUtils.Results`    | `Result<T>`, `Result`, `Unit` — functional result type + chaining extensions                         |
| `CommonUtils.Middleware` | `CommonApiExceptionHandler`, `CorrelationIdMiddleware`, `ICorrelationIdAccessor`                      |
| `CommonUtils.Auth`       | `ClaimsPrincipalExtensions`, `ICurrentUserContext`                                                    |
| `CommonUtils.Linq`       | `QueryableExtensions` — IQueryable pagination, sorting, paged result                                 |
| `CommonUtils.Filters`    | `ValidateModelFilter` — automatic ModelState validation filter                                        |

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

| Method                                | Throws              | Notes                                              |
| ------------------------------------- | ------------------- | -------------------------------------------------- |
| `NotEmpty(string?, paramName)`        | `ArgumentException` | Null, empty, or whitespace. Returns trimmed value. |
| `MaxLength(string, max, paramName)`   | `ArgumentException` | Trims before measuring. Returns original value.    |
| `MinLength(string, min, paramName)`   | `ArgumentException` | Trims before measuring. Returns original value.    |
| `Length(string, min, max, paramName)` | `ArgumentException` | Combined min + max check.                          |

### Numeric guards — `int`, `long`, `decimal`, `double`

| Method                                | Throws                        | Notes                         |
| ------------------------------------- | ----------------------------- | ----------------------------- |
| `Positive(value, paramName)`          | `ArgumentOutOfRangeException` | value > 0                     |
| `NotNegative(value, paramName)`       | `ArgumentOutOfRangeException` | value ≥ 0                     |
| `InRange(value, min, max, paramName)` | `ArgumentOutOfRangeException` | `int` and `decimal` overloads |

### Collection guards

| Method                                                | Throws                  | Notes                                              |
| ----------------------------------------------------- | ----------------------- | -------------------------------------------------- |
| `NotNull<T>(value, paramName)`                        | `ArgumentNullException` | Any reference type                                 |
| `NotEmpty<T>(IEnumerable<T>?, paramName)`             | `ArgumentException`     | Null or empty                                      |
| `NotEmpty<T>(IReadOnlyCollection<T>?, paramName)`     | `ArgumentException`     | Preferred overload — avoids double-enumeration     |
| `MaxCount<T>(ICollection<T>, max, paramName)`         | `ArgumentException`     | —                                                  |
| `MinCount<T>(ICollection<T>, min, paramName)`         | `ArgumentException`     | —                                                  |

### Other guards

| Method                                   | Throws                        | Notes                       |
| ---------------------------------------- | ----------------------------- | --------------------------- |
| `NotEmpty(Guid, paramName)`              | `ArgumentException`           | `Guid.Empty` check          |
| `Defined<T>(T, paramName)`               | `ArgumentException`           | Enum value must be declared |
| `NotDefault(DateTime, paramName)`        | `ArgumentException`           | Rejects `DateTime.MinValue` |
| `NotDefault(DateTimeOffset, paramName)`  | `ArgumentException`           | —                           |
| `NotInPast(DateTime, paramName)`         | `ArgumentOutOfRangeException` | Calls `NotDefault` first    |
| `NotInFuture(DateTime, paramName)`       | `ArgumentOutOfRangeException` | Calls `NotDefault` first    |
| `NotInPast(DateTimeOffset, paramName)`   | `ArgumentOutOfRangeException` | —                           |
| `NotInFuture(DateTimeOffset, paramName)` | `ArgumentOutOfRangeException` | —                           |
| `NotDefault(DateOnly, paramName)`        | `ArgumentException`           | Rejects `DateOnly.MinValue` |
| `NotInPast(DateOnly, paramName)`         | `ArgumentOutOfRangeException` | Calls `NotDefault` first    |
| `NotInFuture(DateOnly, paramName)`       | `ArgumentOutOfRangeException` | Calls `NotDefault` first    |
| `NotDefault(TimeOnly, paramName)`        | `ArgumentException`           | Rejects `TimeOnly.MinValue` |
| `InRange(TimeOnly, min, max, paramName)` | `ArgumentOutOfRangeException` | Inclusive bounds            |
| `Positive(TimeSpan, paramName)`          | `ArgumentOutOfRangeException` | value > Zero                |
| `NotNegative(TimeSpan, paramName)`       | `ArgumentOutOfRangeException` | value ≥ Zero                |
| `InRange(TimeSpan, min, max, paramName)` | `ArgumentOutOfRangeException` | Inclusive bounds            |

```csharp
var id         = Check.NotEmpty(dto.Id, nameof(dto.Id));
var tags       = Check.NotEmpty(dto.Tags, nameof(dto.Tags));
var role       = Check.Defined(dto.Role, nameof(dto.Role));
var expires    = Check.NotInPast(dto.ExpiresAt, nameof(dto.ExpiresAt));
var apptDate   = Check.NotInPast(dto.AppointmentDate, nameof(dto.AppointmentDate)); // DateOnly
var openTime   = Check.InRange(dto.OpenTime, new TimeOnly(8,0), new TimeOnly(20,0), nameof(dto.OpenTime));
var timeout    = Check.Positive(dto.Timeout, nameof(dto.Timeout)); // TimeSpan
```

### Format guards

| Method                                 | Throws              | Notes                                          |
| -------------------------------------- | ------------------- | ---------------------------------------------- |
| `Email(string?, paramName)`            | `ArgumentException` | RFC-style format check. Returns trimmed value. |
| `Url(string?, paramName)`              | `ArgumentException` | Absolute HTTP/HTTPS URL only.                  |
| `Matches(string?, pattern, paramName)` | `ArgumentException` | Custom regex with 1-second timeout.            |
| `Phone(string?, paramName)`            | `ArgumentException` | E.164 format (`+15551234567`).                 |

```csharp
email    = Check.Email(dto.Email, nameof(dto.Email));
website  = Check.Url(dto.Website, nameof(dto.Website));
postCode = Check.Matches(dto.PostCode, @"^\d{5}$", nameof(dto.PostCode));
phone    = Check.Phone(dto.Phone, nameof(dto.Phone));
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

### `ToKebabCase`

Converts PascalCase or camelCase to `kebab-case`. Useful for URL slugs and route segments.

```csharp
"OrderLineItem".ToKebabCase() // "order-line-item"
"createdAt".ToKebabCase()     // "created-at"
```

### `ToPascalCase`

Converts `snake_case` or `kebab-case` to `PascalCase`.

```csharp
"order_line_item".ToPascalCase() // "OrderLineItem"
"order-line-item".ToPascalCase() // "OrderLineItem"
"hello".ToPascalCase()           // "Hello"
```

### `Mask`

Masks the middle of a string, preserving a configurable number of characters at each end. Safe for logging emails, tokens, and phone numbers.

```csharp
"user@example.com".Mask()              // "us**************" (2 visible start, default)
"user@example.com".Mask(2, 3)         // "us***********com"
"secret-token".Mask(0, 0, '#')        // "############"
```

**Parameters:** `visibleStart` (default 2), `visibleEnd` (default 0), `maskChar` (default `*`).  
When `visibleStart + visibleEnd ≥ length`, the entire string is masked.

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

| Property          | Type               | Description                         |
| ----------------- | ------------------ | ----------------------------------- |
| `Items`           | `IReadOnlyList<T>` | Current page items                  |
| `Page`            | `int`              | Current page (1-based)              |
| `PageSize`        | `int`              | Items per page                      |
| `TotalCount`      | `int`              | Total items across all pages        |
| `TotalPages`      | `int`              | Computed: `⌈TotalCount / PageSize⌉` |
| `HasNextPage`     | `bool`             | `Page < TotalPages`                 |
| `HasPreviousPage` | `bool`             | `Page > 1`                          |

Use `PagedResult.Empty<T>(pagination)` when the data source returns nothing.

### `SortParams`

Companion to `PaginationParams` for list endpoints that support ordering. Bind directly from the query string.

```csharp
// GET /orders?sortBy=createdAt&direction=Desc
[HttpGet]
public IActionResult GetOrders(
    [FromQuery] PaginationParams pagination,
    [FromQuery] SortParams sort)
{
    pagination.Validate();
    sort.Validate(["name", "createdAt", "price"]); // throws BadRequestException for unknown columns

    if (sort.IsActive)
    {
        query = sort.Direction == SortDirection.Desc
            ? query.OrderByDescending(sort.SortBy)
            : query.OrderBy(sort.SortBy);
    }
}
```

`Validate(allowedColumns)` is a no-op when `SortBy` is null or empty — safe to call unconditionally.

### `MultiSortParams`

For endpoints that need compound ordering across multiple columns. Bind from repeated query string values.

```csharp
// GET /orders?sort=createdAt:desc&sort=name:asc
[HttpGet]
public IActionResult GetOrders(
    [FromQuery] PaginationParams pagination,
    [FromQuery] MultiSortParams sort)
{
    sort.Validate(["name", "createdAt", "price"]); // throws BadRequestException for unknown columns

    foreach (var criterion in sort.Criteria)
    {
        query = criterion.Direction == SortDirection.Desc
            ? query.OrderByDescending(criterion.SortBy)
            : query.OrderBy(criterion.SortBy);
    }
}
```

Each entry in `Sort` is a `"column:direction"` string. Direction defaults to `asc` when omitted.

### `CursorPaginationParams<TCursor>`

Keyset/cursor pagination for large datasets — avoids the `OFFSET n` performance cliff at high page numbers.

```csharp
// GET /orders?after=550&pageSize=20
[HttpGet]
public async Task<IActionResult> GetOrders(
    [FromQuery] CursorPaginationParams<int?> cursor)
{
    cursor.Validate(); // throws BadRequestException when PageSize < 1 or > 100

    var items = await _db.Orders
        .Where(o => cursor.After == null || o.Id > cursor.After)
        .OrderBy(o => o.Id)
        .Take(cursor.PageSize + 1)
        .ToListAsync();

    var hasMore = items.Count > cursor.PageSize;
    if (hasMore) items.RemoveAt(items.Count - 1);

    var result = CursorPagedResult.Create<Order, int?>(
        items.AsReadOnly(),
        hasMore ? items[^1].Id : null);

    return Ok(ApiResponse.Ok(result));
}
```

---

## Middleware

### `CommonApiExceptionHandler`

Shared `IExceptionHandler` that maps `ApiException` subclasses to uniform JSON responses. Eliminates the per-project `UseExceptionHandler` boilerplate.

```csharp
// Program.cs
builder.Services.AddCommonApiExceptionHandling();  // or .AddCommonApiExceptionHandling(useProblemDetails: true)

app.UseCommonApiExceptionHandling();
```

Supports two modes:
- **Default** — returns `ApiResponse`-shaped JSON (`{ success, errors, errorCode }`)
- **ProblemDetails** — returns RFC 7807 `application/problem+json`

`TooManyRequestsException` automatically sets the `Retry-After` response header.

### `CorrelationIdMiddleware`

Reads or generates a `X-Correlation-ID` for every request, making it available to downstream services via `ICorrelationIdAccessor`. The ID is echoed back in the response header.

```csharp
// Program.cs
builder.Services.AddCorrelationId();       // registers middleware + ICorrelationIdAccessor
app.UseCorrelationId();

// Inject in a service or controller
public class OrderService(ICorrelationIdAccessor correlationId)
{
    public void LogSomething()
    {
        _logger.LogInformation("CorrelationId: {Id}", correlationId.CorrelationId);
    }
}
```

Custom header name:

```csharp
builder.Services.AddCorrelationId(options => options.HeaderName = "X-Request-ID");
```

---

## IQueryable Extensions

```csharp
using CommonUtils.Linq;
```

Eliminates manual `Skip`/`Take`/`OrderBy` in every service method. The `ToPagedResultAsync` overload accepts a materializer delegate so the library stays free of an EF Core dependency.

```csharp
// Apply pagination and sorting, then materialise
var result = await _db.Orders
    .Where(o => o.CustomerId == customerId)
    .ApplySorting(sort, new Dictionary<string, Expression<Func<Order, object?>>>
    {
        ["name"]      = o => o.Name,
        ["createdAt"] = o => o.CreatedAt,
    })
    .ToPagedResultAsync(
        pagination,
        materialize: (q, ct) => q.ToListAsync(ct),
        countAsync:  (q, ct) => q.CountAsync(ct),
        cancellationToken);
```

| Method                                                         | Description                                   |
| -------------------------------------------------------------- | --------------------------------------------- |
| `ApplyPagination(PaginationParams)`                            | Applies `Skip` + `Take`                       |
| `ApplySorting(SortParams, columnMap)`                          | Applies type-safe `OrderBy`/`OrderByDesc`     |
| `ToPagedResultAsync(pagination, materialize, countAsync, ct)`  | Counts + pages + wraps in `PagedResult<T>`    |

---

## Auth Helpers

### `ClaimsPrincipalExtensions`

```csharp
using CommonUtils.Auth;

// In a controller or service
string? userId = User.GetUserId();   // ClaimTypes.NameIdentifier
string? email  = User.GetEmail();
string? role   = User.GetRole();     // first role claim
IEnumerable<string> roles = User.GetRoles();
string? custom = User.GetClaim("tenant-id");
bool isAdmin   = User.HasClaim(ClaimTypes.Role, "Admin");
```

### `ICurrentUserContext`

Scoped service that wraps the current user's claims for injection into services (no need to pass `ClaimsPrincipal` down the stack).

```csharp
// Program.cs
builder.Services.AddCurrentUserContext();

// In a service
public class OrderService(ICurrentUserContext currentUser)
{
    public void PlaceOrder()
    {
        var userId = currentUser.UserId;      // string?
        var email  = currentUser.Email;       // string?
        var role   = currentUser.Role;        // string?
        var auth   = currentUser.IsAuthenticated; // bool
    }
}
```

---

## Validation Filter

`ValidateModelFilter` is an `ActionFilterAttribute` that automatically throws `ValidationException` when `ModelState` is invalid, removing the need for `if (!ModelState.IsValid)` in every action.

```csharp
// Register globally in Program.cs
builder.Services.AddControllers(options =>
    options.Filters.Add<ValidateModelFilter>());

// Or per-controller / per-action
[ValidateModelFilter]
public IActionResult CreateOrder([FromBody] CreateOrderDto dto) { ... }
```

`ValidationException.FromModelState(ModelState)` is also available directly if you need manual control.

---

## Exceptions

All exceptions extend `ApiException`, which carries an HTTP `StatusCode` and an optional machine-readable `ErrorCode`. Use a single exception-handling middleware to map them to responses — no per-endpoint try/catch needed.

```csharp
using CommonUtils.Exceptions;
```

### Available exceptions

| Class                         | Status | When to use                                       |
| ----------------------------- | ------ | ------------------------------------------------- |
| `BadRequestException`         | 400    | Malformed input, failed preconditions             |
| `UnauthorizedException`       | 401    | Missing or invalid credentials                    |
| `ForbiddenException`          | 403    | Authenticated but not permitted                   |
| `NotFoundException`           | 404    | Resource does not exist                           |
| `ConflictException`           | 409    | Duplicate or state conflict                       |
| `ValidationException`         | 422    | Semantic validation errors (field-level)          |
| `TooManyRequestsException`    | 429    | Rate limit exceeded                               |
| `GoneException`               | 410    | Resource permanently removed                      |
| `ServiceUnavailableException` | 503    | Dependency down, maintenance, or transient outage |

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

### New exceptions usage

```csharp
// Rate limiting — include a retry hint for the client
throw new TooManyRequestsException(retryAfter: TimeSpan.FromSeconds(30));

// Map RetryAfter in middleware
if (ex is TooManyRequestsException tooMany && tooMany.RetryAfter.HasValue)
    context.Response.Headers["Retry-After"] = ((int)tooMany.RetryAfter.Value.TotalSeconds).ToString();

// Permanently removed resource
throw new GoneException("This API version has been retired.");

// Dependency or database unavailable
throw new ServiceUnavailableException("Payment provider is currently unavailable.", "PAYMENT_DOWN");
```

### Middleware integration (ASP.NET Core)

Use `CommonApiExceptionHandler` from `CommonUtils.Middleware` for zero-boilerplate setup (see [Middleware](#middleware) section above), or wire it manually:

```csharp
app.UseExceptionHandler(builder => builder.Run(async context =>
{
    var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;

    if (ex is ApiException apiEx)
    {
        context.Response.StatusCode = apiEx.StatusCode;

        if (ex is TooManyRequestsException tooMany && tooMany.RetryAfter.HasValue)
            context.Response.Headers["Retry-After"] = ((int)tooMany.RetryAfter.Value.TotalSeconds).ToString();

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

## Result\<T\> — Functional Result Type

An alternative to throwing exceptions for expected failure paths. Operations return `Result<T>` — the caller decides what to do with a failure rather than catching an exception.

```csharp
using CommonUtils.Results;
```

### Returning results

```csharp
// Success with a value
Result<Order> result = Result.Ok(order);

// Failure with a single error
Result<Order> result = Result.Fail<Order>("Order not found.");

// Failure with multiple errors
Result<Order> result = Result.Fail<Order>(["Stock too low", "Payment declined"]);

// No-value success (commands, void operations)
Result<Unit> result = Result.Ok();

// Implicit conversion from value
Result<int> result = 42; // equivalent to Result.Ok(42)
```

### Consuming results

```csharp
var result = _orderService.PlaceOrder(dto);

if (result.IsFailure)
    return BadRequest(ApiResponse.Fail<Order>(result.Errors));

return Ok(ApiResponse.Ok(result.Value!));
```

### Chaining — `ResultExtensions`

Build pipelines without manual `if (result.IsFailure)` checks:

```csharp
using CommonUtils.Results;

var response = await _repo.FindOrderAsync(id)      // Task<Result<Order>>
    .MapAsync(order => new OrderDto(order))         // project value
    .BindAsync(dto => _validator.ValidateAsync(dto)) // chain another result
    .MatchAsync(
        onSuccess: dto  => Ok(ApiResponse.Ok(dto)),
        onFailure: errs => BadRequest(ApiResponse.Fail<OrderDto>(errs)));
```

| Method                               | Description                                                  |
| ------------------------------------ | ------------------------------------------------------------ |
| `Map(Func<T, TOut>)`                 | Project value; forwards errors unchanged                     |
| `Bind(Func<T, Result<TOut>>)`        | Chain a result-returning function                            |
| `Match(onSuccess, onFailure)`        | Collapse both paths into a single value                      |
| `OnSuccess(Action<T>)`               | Side-effect on success; returns original result              |
| `OnFailure(Action<errors>)`          | Side-effect on failure; returns original result              |
| `Recover(Func<errors, T>)`           | Supply a fallback value on failure                           |
| `ToApiResponse(message?)`            | Convert to `ApiResponse<T>`                                  |
| `ToActionResult(message?)`           | Convert to `OkObjectResult` or `BadRequestObjectResult`      |
| `MapAsync` / `BindAsync` / `MatchAsync` | Async equivalents for `Task<Result<T>>` pipelines         |

### Properties on `Result<T>`

| Property    | Type                    | Description                             |
| ----------- | ----------------------- | --------------------------------------- |
| `IsSuccess` | `bool`                  | `true` when the operation succeeded     |
| `IsFailure` | `bool`                  | `!IsSuccess`                            |
| `Value`     | `T?`                    | The result value; meaningful on success |
| `Errors`    | `IReadOnlyList<string>` | Error messages; empty on success        |

Use `Result<Unit>` for operations with no return value. `Unit.Value` is the singleton instance.

---

## License

MIT — see [LICENSE.txt](LICENSE.txt).
