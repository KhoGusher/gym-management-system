using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Members.Commands.UpdateMember;

public class UpdateMemberCommandHandler : IRequestHandler<UpdateMemberCommand, bool>
{
    private readonly IAppDbContext _dbContext;

    public UpdateMemberCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<bool> Handle(UpdateMemberCommand request, CancellationToken cancellationToken)
    {
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken);

        if (member is null) return false;

        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.Email = request.Email;
        member.Phone = request.Phone;
        member.DateOfBirth = request.DateOfBirth;
        // UpdatedAtUtc gets stamped automatically by AppDbContext.SaveChangesAsync — we never set it here.

        await _dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}