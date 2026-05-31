using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommonUtils.Middleware;

/// <summary>
/// Extension methods for registering and enabling correlation ID tracking.
/// </summary>
public static class CorrelationIdExtensions
{
    /// <summary>
    /// Registers <see cref="CorrelationIdMiddleware"/> and <see cref="ICorrelationIdAccessor"/>
    /// with the DI container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional delegate to configure <see cref="CorrelationIdOptions"/>.</param>
    public static IServiceCollection AddCorrelationId(
        this IServiceCollection services,
        Action<CorrelationIdOptions>? configure = null)
    {
        var options = new CorrelationIdOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddScoped<CorrelationIdMiddleware>();

        return services;
    }

    /// <summary>
    /// Adds the correlation ID middleware to the request pipeline.
    /// Should be placed near the top of the pipeline, before logging or auth.
    /// </summary>
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app) =>
        app.UseMiddleware<CorrelationIdMiddleware>();
}
