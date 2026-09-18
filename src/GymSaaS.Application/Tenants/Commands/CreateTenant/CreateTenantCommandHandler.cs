using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Domain.Entities;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreateTenantCommandHandler(IAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = new Tenant
        {
            CompanyName = request.CompanyName,
            SubdomainSlug = request.SubdomainSlug,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone
        };

        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return tenant.Id;
    }
}