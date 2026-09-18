using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Entities;

namespace GymSaaS.Application.Subscriptions.Commands.CreatePackage;

public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreatePackageCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<Guid> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = new SubscriptionPackage
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            DurationInDays = request.DurationInDays
        };

        _dbContext.SubscriptionPackages.Add(package);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return package.Id;
    }
}