using GymSaaS.Application.Common.Mediator;
using GymSaaS.Domain.Enums;

namespace GymSaaS.Application.Attendance.Commands.CheckIn;

public record CheckInCommand(Guid MemberId, CheckInMethod Method) : IRequest<CheckInResultDto>;

public record CheckInResultDto(Guid AttendanceId, string MemberFullName, DateTime CheckInUtc, int DaysRemainingOnSubscription);