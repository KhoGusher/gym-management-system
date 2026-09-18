using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Subscriptions.Queries.GetPackages;

public record PackageDto(Guid Id, string Name, string? Description, decimal Price, int DurationInDays, bool IsActive);

public record GetPackagesQuery(bool ActiveOnly = true) : IRequest<IReadOnlyList<PackageDto>>;