using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using ResNet.Domain.Constants;

namespace Infrastructure.Data.Seeder;

public static class DefaultRoles
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new List<string>()
        {
            Roles.Admin,
            Roles.Cashier,
        };

        foreach (var role in roles)
        {
            var roleExist = await roleManager.RoleExistsAsync(role);
            if (roleExist)
            {
                continue;
            }
            
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}