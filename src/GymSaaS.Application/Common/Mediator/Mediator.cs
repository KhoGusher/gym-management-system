using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace GymSaaS.Application.Common.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();

        // Build the closed generic type IRequestHandler<CreateTenantCommand, Guid> at runtime
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType)
            ?? throw new InvalidOperationException($"No handler registered for {requestType.Name}");

        // The actual call into the handler — wrapped as a delegate so behaviors can wrap around it
        RequestHandlerDelegate<TResponse> handlerDelegate = () =>
        {
            var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.Handle))!;
            return (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
        };

        // Find every IPipelineBehavior<TRequest, TResponse> registered for this specific request type
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviors = _serviceProvider.GetServices(behaviorType).Cast<object>().Reverse().ToList();

        // Wrap the handler delegate in each behavior, innermost first — same pattern as ASP.NET Core middleware
        var pipeline = handlerDelegate;
        foreach (var behavior in behaviors)
        {
            var next = pipeline;
            var method = behaviorType.GetMethod(nameof(IPipelineBehavior<IRequest<TResponse>, TResponse>.Handle))!;
            pipeline = () => (Task<TResponse>)method.Invoke(behavior, new object[] { request, next, cancellationToken })!;
        }

        return await pipeline();
    }
}