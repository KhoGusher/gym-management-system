namespace GymSaaS.Application.Common.Interfaces;

public record AuthResult(bool Succeeded, string? AccessToken, string? RefreshToken, IEnumerable<string>? Errors = null);

public interface IIdentityService
{
    Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, Guid tenantId, string role);
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult> RefreshTokenAsync(string refreshToken);
    Task<IReadOnlyList<UserSummaryDto>> GetUsersByTenantAsync(Guid tenantId);
}

public record UserSummaryDto(Guid Id, string Email, string FirstName, string LastName, IReadOnlyList<string> Roles, bool IsActive);

