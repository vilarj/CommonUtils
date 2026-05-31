using Microsoft.AspNetCore.Http;

namespace CommonUtils.Auth;

/// <summary>
/// Default <see cref="ICurrentUserContext"/> implementation that reads claims from
/// the <see cref="HttpContext"/> via <see cref="IHttpContextAccessor"/>.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class HttpContextCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public HttpContextCurrentUserContext(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private System.Security.Claims.ClaimsPrincipal? Principal =>
        _accessor.HttpContext?.User;

    public string? UserId => Principal?.GetUserId();
    public string? Email => Principal?.GetEmail();
    public string? Role => Principal?.GetRole();
    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated is true;
}
