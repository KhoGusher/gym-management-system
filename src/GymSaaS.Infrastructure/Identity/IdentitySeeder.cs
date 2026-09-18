using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymSaaS.Infrastructure.Identity;

public static class IdentitySeeder
{
    public static async Task SeedSuperAdminAsync(IServiceProvider services, IConfiguration configuration)
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        const string role = "SuperAdmin";
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));

        var email = configuration["SuperAdmin:Email"];
        var password = configuration["SuperAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return; // not configured — skip silently rather than crash on every startup

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return; // already seeded

        var superAdmin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "Gusherlabs",
            LastName = "Admin",
            TenantId = null // SuperAdmin is not scoped to any tenant
        };

        var result = await userManager.CreateAsync(superAdmin, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(superAdmin, role);
    }
}