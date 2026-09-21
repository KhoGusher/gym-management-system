using GymSaaS.Domain.Common;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Domain.Entities;

public class AttendanceRecord : BaseEntity, ITenantScoped
{
    public Guid TenantId { get; set; }

    public required Guid MemberId { get; set; }
    public Member? Member { get; set; }

    public required CheckInMethod Method { get; set; }
    public DateTime CheckInUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CheckOutUtc { get; set; }

    // What subscription was active at the moment of check-in — an audit trail,
    // so a dispute later ("why was I let in?") can be answered from history, not guessed.
    public Guid? VerifiedSubscriptionId { get; set; }

    public TimeSpan? Duration => CheckOutUtc.HasValue ? CheckOutUtc.Value - CheckInUtc : null;
}