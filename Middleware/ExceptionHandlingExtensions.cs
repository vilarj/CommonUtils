using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CommonUtils.Middleware;

/// <summary>
/// Extension methods for registering and enabling CommonUtils exception handling.
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// Registers the <see cref="CommonApiExceptionHandler"/> with the DI container.
    /// Call this before <see cref="UseCommonApiExceptionHandling"/>.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional delegate to configure <see cref="ExceptionHandlingOptions"/>.</param>
    public static IServiceCollection AddCommonApiExceptionHandling(
        this IServiceCollection services,
        Action<ExceptionHandlingOptions>? configure = null)
    {
        var options = new ExceptionHandlingOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddExceptionHandler<CommonApiExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    /// <summary>
    /// Adds the CommonUtils exception-handling middleware to the request pipeline.
    /// Call <see cref="AddCommonApiExceptionHandling"/> in <c>Program.cs</c> first.
    /// </summary>
    public static IApplicationBuilder UseCommonApiExceptionHandling(this IApplicationBuilder app) =>
        app.UseExceptionHandler();
}
