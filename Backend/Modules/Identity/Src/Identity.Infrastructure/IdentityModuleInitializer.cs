using BuildingBlocks.Infrastructure;
using Identity.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Infrastructure;

public class IdentityModuleInitializer(
    RoleManager<IdentityRole> roleManager,
    ILogger<IdentityModuleInitializer> logger) : IModuleInitializer
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Initializing Identity Module");

        logger.LogInformation("Seeding roles...");

        string[] roleNames = [Roles.Admin, Roles.User, Roles.Organizer];
        var storedRoles = await roleManager.Roles.ToListAsync(ct);

        foreach (var roleName in roleNames)
            if (storedRoles.All(r => r.Name != roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (result.Succeeded) continue;

                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to seed role '{roleName}'. Errors: {errors}");
            }

        logger.LogInformation("Roles seeded successfully.");
    }
}