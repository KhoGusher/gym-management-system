using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Subscriptions.Commands.CreatePackage;

public record CreatePackageCommand(string Name, string? Description, decimal Price, int DurationInDays) : IRequest<Guid>;