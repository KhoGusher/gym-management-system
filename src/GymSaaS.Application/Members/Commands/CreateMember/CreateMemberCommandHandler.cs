using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Domain.Entities;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Members.Commands.CreateMember;

public class CreateMemberCommandHandler : IRequestHandler<CreateMemberCommand, Guid>
{
    private readonly IAppDbContext _dbContext;

    public CreateMemberCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<Guid> Handle(CreateMemberCommand request, CancellationToken cancellationToken)
    {
        // Note: we do NOT set TenantId here manually.
        // AppDbContext.SaveChangesAsync auto-assigns it from ICurrentTenantService on insert — remember that override we wrote.
        var member = new Member
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth
        };

        _dbContext.Members.Add(member);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return member.Id;
    }
}