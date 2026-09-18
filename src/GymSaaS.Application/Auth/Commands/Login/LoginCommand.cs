using GymSaaS.Application.Auth.Commands.Register;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;