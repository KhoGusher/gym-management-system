using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GymSaaS.Application.Common.Mediator;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomMediator(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IMediator, Mediator>();

        RegisterOpenGenericImplementations(services, assembly, typeof(IRequestHandler<,>));
        RegisterOpenGenericImplementations(services, assembly, typeof(IPipelineBehavior<,>));

        return services;
    }

    private static void RegisterOpenGenericImplementations(IServiceCollection services, Assembly assembly, Type openGenericInterface)
    {
        var matches = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
            .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == openGenericInterface);

        foreach (var match in matches)
        {
            // If the implementation itself is generic (like ValidationBehavior<,>), register it
            // using the TRUE open generic type definitions on both sides — required for the
            // container to correctly close it later with real types at resolution time.
            if (match.Implementation.IsGenericTypeDefinition)
            {
                services.AddTransient(openGenericInterface, match.Implementation);
            }
            else
            {
                // Concrete handler (e.g. CreateTenantCommandHandler) — already fully closed, register as-is.
                services.AddTransient(match.Interface, match.Implementation);
            }
        }
    }
}