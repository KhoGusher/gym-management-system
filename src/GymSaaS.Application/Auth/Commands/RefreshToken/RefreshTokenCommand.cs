using GymSaaS.Application.Auth.Commands.Register;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;