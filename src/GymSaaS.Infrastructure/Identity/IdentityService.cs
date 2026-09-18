using GymSaaS.Application.Common.Interfaces;
using GymSaaS.Domain.Entities;
using GymSaaS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymSaaS.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly TokenService _tokenService;
    private readonly AppDbContext _dbContext; // direct concrete reference is fine here — this class lives in Infrastructure already

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        TokenService tokenService,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _dbContext = dbContext;
    }

    public async Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName, Guid tenantId, string role)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
            return new AuthResult(false, null, null, new[] { "A user with this email already exists." });

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            TenantId = tenantId
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
            return new AuthResult(false, null, null, createResult.Errors.Select(e => e.Description));

        if (!await _roleManager.RoleExistsAsync(role))
            await _roleManager.CreateAsync(new IdentityRole<Guid>(role));

        await _userManager.AddToRoleAsync(user, role);

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !user.IsActive)
            return new AuthResult(false, null, null, new[] { "Invalid credentials." });

        var validPassword = await _userManager.CheckPasswordAsync(user, password);
        if (!validPassword)
            return new AuthResult(false, null, null, new[] { "Invalid credentials." });

        return await IssueTokensAsync(user);
    }

    public async Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        var existingToken = await _dbContext.RefreshTokens
            .IgnoreQueryFilters() // RefreshToken isn't ITenantScoped, but being explicit/safe here
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (existingToken is null)
            return new AuthResult(false, null, null, new[] { "Invalid refresh token." });

        if (!existingToken.IsActive)
        {
            // Token reuse detection: if a REVOKED token is presented again, it may be stolen.
            // Defensive move: revoke the entire chain for that user.
            if (existingToken.RevokedAtUtc is not null)
            {
                var userTokens = await _dbContext.RefreshTokens
                    .Where(rt => rt.UserId == existingToken.UserId && rt.RevokedAtUtc == null)
                    .ToListAsync();
                foreach (var t in userTokens) t.RevokedAtUtc = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync(CancellationToken.None);
            }

            return new AuthResult(false, null, null, new[] { "Refresh token is no longer valid." });
        }

        var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());
        if (user is null || !user.IsActive)
            return new AuthResult(false, null, null, new[] { "User not found or inactive." });

        // Rotate: revoke old, issue new
        var newTokenResult = await IssueTokensAsync(user);
        existingToken.RevokedAtUtc = DateTime.UtcNow;
        existingToken.ReplacedByToken = newTokenResult.RefreshToken;
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return newTokenResult;
    }

    private async Task<AuthResult> IssueTokensAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(_tokenService.RefreshTokenExpiryDays)
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        return new AuthResult(true, accessToken, refreshTokenValue);
    }

    public async Task<IReadOnlyList<UserSummaryDto>> GetUsersByTenantAsync(Guid tenantId)
    {
        var users = await _userManager.Users.Where(u => u.TenantId == tenantId).ToListAsync();

        var result = new List<UserSummaryDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserSummaryDto(user.Id, user.Email!, user.FirstName, user.LastName, roles.ToList(), user.IsActive));
        }
        return result;
    }
}