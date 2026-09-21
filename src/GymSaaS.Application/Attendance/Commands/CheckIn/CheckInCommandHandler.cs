using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Entities;
using GymSaaS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Application.Attendance.Commands.CheckIn;

public class CheckInCommandHandler : IRequestHandler<CheckInCommand, CheckInResultDto>
{
    private readonly IAppDbContext _dbContext;

    public CheckInCommandHandler(IAppDbContext dbContext) => _dbContext = dbContext;

    public async Task<CheckInResultDto> Handle(CheckInCommand request, CancellationToken cancellationToken)
    {
        var member = await _dbContext.Members
            .FirstOrDefaultAsync(m => m.Id == request.MemberId, cancellationToken)
            ?? throw new KeyNotFoundException("Member not found.");

        // Payment verification gate — no active, unexpired subscription, no entry.
        var activeSubscription = await _dbContext.MemberSubscriptions
            .Where(ms => ms.MemberId == request.MemberId
                      && ms.Status == Domain.Enums.SubscriptionStatus.Active
                      && ms.EndDateUtc > DateTime.UtcNow)
            .OrderByDescending(ms => ms.EndDateUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (activeSubscription is null)
            throw new InvalidOperationException($"{member.FullName} has no active subscription. Check-in denied.");

        // Prevent double check-in: if they're already checked in without a checkout, block a second entry.
        var alreadyCheckedIn = await _dbContext.AttendanceRecords
            .AnyAsync(a => a.MemberId == request.MemberId && a.CheckOutUtc == null, cancellationToken);

        if (alreadyCheckedIn)
            throw new InvalidOperationException($"{member.FullName} is already checked in.");

        var attendance = new AttendanceRecord
        {
            MemberId = member.Id,
            Method = request.Method,
            VerifiedSubscriptionId = activeSubscription.Id
        };

        _dbContext.AttendanceRecords.Add(attendance);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var daysRemaining = (activeSubscription.EndDateUtc - DateTime.UtcNow).Days;

        return new CheckInResultDto(attendance.Id, member.FullName, attendance.CheckInUtc, daysRemaining);
    }
}