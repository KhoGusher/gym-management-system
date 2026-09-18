using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Tenants.Commands.CreateTenant;

// IRequest<TResponse> — this record IS the "message" sent to MediatR.
// Whatever handler is registered for CreateTenantCommand will process it and return a Guid (the new Tenant's Id).
public record CreateTenantCommand(
    string CompanyName,
    string SubdomainSlug,
    string? ContactEmail,
    string? ContactPhone
) : IRequest<Guid>;