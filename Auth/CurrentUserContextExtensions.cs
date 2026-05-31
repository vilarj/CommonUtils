using Microsoft.Extensions.DependencyInjection;

namespace CommonUtils.Auth;

/// <summary>
/// Extension methods for registering current-user utilities with the DI container.
/// </summary>
public static class CurrentUserContextExtensions
{
    /// <summary>
    /// Registers <see cref="ICurrentUserContext"/> as a scoped service backed by
    /// <see cref="IHttpContextAccessor"/>. Also ensures <c>IHttpContextAccessor</c>
    /// is registered.
    /// </summary>
    public static IServiceCollection AddCurrentUserContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserContext, HttpContextCurrentUserContext>();
        return services;
    }
}
