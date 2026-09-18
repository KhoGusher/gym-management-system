using GymSaaS.Application.Auth.Commands.Register;
using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Application.Common.Mediator;

namespace GymSaaS.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _identityService.RefreshTokenAsync(request.RefreshToken);

        if (!result.Succeeded)
            throw new UnauthorizedAccessException(string.Join("; ", result.Errors ?? Array.Empty<string>()));

        return new AuthResponseDto(result.AccessToken!, result.RefreshToken!);
    }
}