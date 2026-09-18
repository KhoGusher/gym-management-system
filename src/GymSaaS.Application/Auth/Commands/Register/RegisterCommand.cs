using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Auth.Commands.Register;

public record AuthResponseDto(string AccessToken, string RefreshToken);

public record RegisterCommand(
    string Email, string Password, string FirstName, string LastName, Guid TenantId, string Role
) : IRequest<AuthResponseDto>;