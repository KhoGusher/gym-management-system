using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RegisterAsync(
            request.Email, request.Password, request.FirstName, request.LastName, request.TenantId, request.Role);

        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors ?? Array.Empty<string>()));

        return new AuthResponseDto(result.AccessToken!, result.RefreshToken!);
    }
}