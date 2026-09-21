using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Attendance.Commands.CheckOut;

public record CheckOutCommand(Guid MemberId) : IRequest<bool>;