using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;
using System;

namespace Infrastructure.Data.Seeder;

public static class DefaultUser
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        var user = new ApplicationUser
        {
            UserName = "Yusuf",
            FullName = "Shobadalov Yusuf",
            Email = "shobadalovyusuf018@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "985565675",
        };
        
        var existingUser = await userManager.FindByNameAsync(user.UserName);
        if (existingUser != null)
        {
            Console.WriteLine("Default user уже существует.");
            return;
        }

        var result = await userManager.CreateAsync(user, "Admin123");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Roles.Admin);
            Console.WriteLine("Default user успешно создан.");
        }
        else
        {
            Console.WriteLine("Ошибка при создании default user:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($" - {error.Code}: {error.Description}");
            }
        }
    }
}
