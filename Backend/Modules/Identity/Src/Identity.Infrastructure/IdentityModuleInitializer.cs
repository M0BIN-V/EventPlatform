using BuildingBlocks.Infrastructure;
using Identity.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure;

public class IdentityModuleInitializer(RoleManager<IdentityRole> roleManager) : IModuleInitializer
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
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
    }
}