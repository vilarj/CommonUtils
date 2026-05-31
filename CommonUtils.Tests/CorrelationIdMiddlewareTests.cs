using CommonUtils.Middleware;
using Microsoft.AspNetCore.Http;

namespace CommonUtils.Tests;

public class CorrelationIdMiddlewareTests
{
    private static CorrelationIdMiddleware CreateMiddleware(
        CorrelationIdAccessor accessor,
        string? headerName = null)
    {
        var options = new CorrelationIdOptions();
        if (headerName is not null)
            options.HeaderName = headerName;
        return new CorrelationIdMiddleware(options, accessor);
    }

    // ── Uses existing header ──────────────────────────────────────────────────

    [Fact]
    public async Task InvokeAsync_WithExistingHeader_UsesItAsCorrelationId()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor);

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Correlation-ID"] = "existing-id";

        await middleware.InvokeAsync(ctx, _ => Task.CompletedTask);

        Assert.Equal("existing-id", accessor.CorrelationId);
    }

    // ── Generates new ID when header absent ──────────────────────────────────

    [Fact]
    public async Task InvokeAsync_WithoutHeader_GeneratesId()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor);

        var ctx = new DefaultHttpContext();
        await middleware.InvokeAsync(ctx, _ => Task.CompletedTask);

        Assert.False(string.IsNullOrWhiteSpace(accessor.CorrelationId));
    }

    // ── Generates new ID when header is whitespace ────────────────────────────

    [Fact]
    public async Task InvokeAsync_WithWhitespaceHeader_GeneratesId()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor);

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Correlation-ID"] = "   ";

        await middleware.InvokeAsync(ctx, _ => Task.CompletedTask);

        Assert.False(string.IsNullOrWhiteSpace(accessor.CorrelationId));
        Assert.NotEqual("   ", accessor.CorrelationId);
    }

    // ── Respects custom header name ───────────────────────────────────────────

    [Fact]
    public async Task InvokeAsync_CustomHeaderName_ReadsFromCustomHeader()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor, headerName: "X-Request-ID");

        var ctx = new DefaultHttpContext();
        ctx.Request.Headers["X-Request-ID"] = "custom-id";

        await middleware.InvokeAsync(ctx, _ => Task.CompletedTask);

        Assert.Equal("custom-id", accessor.CorrelationId);
    }

    // ── Calls next delegate ───────────────────────────────────────────────────

    [Fact]
    public async Task InvokeAsync_AlwaysCallsNextDelegate()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor);
        var nextCalled = false;

        await middleware.InvokeAsync(new DefaultHttpContext(), _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        Assert.True(nextCalled);
    }

    // ── Null guards ───────────────────────────────────────────────────────────

    [Fact]
    public async Task InvokeAsync_NullContext_ThrowsArgumentNullException()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor);
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            middleware.InvokeAsync(null!, _ => Task.CompletedTask));
    }

    [Fact]
    public async Task InvokeAsync_NullNext_ThrowsArgumentNullException()
    {
        var accessor = new CorrelationIdAccessor();
        var middleware = CreateMiddleware(accessor);
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            middleware.InvokeAsync(new DefaultHttpContext(), null!));
    }
}
