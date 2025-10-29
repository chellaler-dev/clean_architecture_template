using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Application.Behaviors;

namespace Application;

public static class DependencyInjection
{
    // Convenience method
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
