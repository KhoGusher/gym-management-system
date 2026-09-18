using GymSaaS.Application.Auth.Commands.Register;
using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.LoginAsync(request.Email, request.Password);

        if (!result.Succeeded)
            throw new UnauthorizedAccessException(string.Join("; ", result.Errors ?? Array.Empty<string>()));

        return new AuthResponseDto(result.AccessToken!, result.RefreshToken!);
    }
}