using FluentValidation;

namespace GymSaaS.Application.Attendance.Commands.CheckIn;

public class CheckInCommandValidator : AbstractValidator<CheckInCommand>
{
    public CheckInCommandValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
    }
}