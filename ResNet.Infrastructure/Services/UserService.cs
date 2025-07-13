using System.Net;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;

public class UserService(
    UserManager<ApplicationUser> userManager,
    DataContext context,
    IMapper mapper,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<PagedResponse<List<GetUserDto>>> GetAllUsersAsync(UserFilter filter)
    {
        logger.LogInformation("GetAllUsersAsync called");

        var users = await userManager.Users.ToListAsync();
        var userDtos = new List<GetUserDto>();

        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);

            userDtos.Add(new GetUserDto
            {
                Id = u.Id,
                Username = u.UserName ?? string.Empty,
                FullName = u.GetType().GetProperty("FullName") != null
                    ? (string)(u.GetType().GetProperty("FullName")?.GetValue(u) ?? "")
                    : "",
                Email = u.Email ?? string.Empty,
                ImageUrl = u.ImageUrl ?? string.Empty,
                PhoneNumber = u.PhoneNumber ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                CreatedAt = u.LockoutEnd?.DateTime ?? DateTime.UtcNow
            });
        }

        if (!string.IsNullOrWhiteSpace(filter.Username))
        {
            userDtos = userDtos
                .Where(u => u.Username.Contains(filter.Username, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        int totalCount = userDtos.Count;
        var pagedData = userDtos
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToList();

        return new PagedResponse<List<GetUserDto>>(pagedData, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetUserDto>> GetUserByIdAsync(string id)
    {
        logger.LogInformation("GetUserByIdAsync called with id={Id}", id);

        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            logger.LogWarning("User with id={Id} not found", id);
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "User not found");
        }

        var roles = await userManager.GetRolesAsync(user);
        var dto = new GetUserDto
        {
            Id = user.Id.ToString(),
            Username = user.UserName ?? "",
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            ImageUrl = user.ImageUrl ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            Role = roles.FirstOrDefault() ?? "",
            CreatedAt = user.LockoutEnd?.DateTime ?? DateTime.UtcNow
        };

        return new Response<GetUserDto>(dto);
    }

    public async Task<Response<GetUserDto>> UpdateUserAsync(string id, UpdateUserDto dto)
    {
        logger.LogInformation("UpdateUserAsync called with id={Id}", id);

        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            logger.LogWarning("User with id={Id} not found", id);
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "User not found");
        }

        user.UserName = dto.Username;
        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var error = updateResult.Errors.FirstOrDefault()?.Description ?? "Failed to update user";
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, error);
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, dto.Role);

        var updated = new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName ?? "",
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            ImageUrl = user.ImageUrl ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            Role = dto.Role,
            CreatedAt = user.LockoutEnd?.DateTime ?? DateTime.UtcNow
        };

        return new Response<GetUserDto>(updated);
    }

    public async Task<Response<string>> DeleteUserAsync(string id)
    {
        logger.LogInformation("DeleteUserAsync called with id={Id}", id);

        var user = await context.Users.FindAsync(id);
        if (user == null)
        {
            logger.LogWarning("User with id={Id} not found", id);
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Failed to delete user";
            return new Response<string>(HttpStatusCode.BadRequest, error);
        }

        return new Response<string>("User deleted successfully");
    }

    public async Task<Response<string>> ChangeUserRoleAsync(string userId, string newRole)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        var currentRoles = await userManager.GetRolesAsync(user);
        var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to remove current roles");

        var addResult = await userManager.AddToRoleAsync(user, newRole);
        if (!addResult.Succeeded)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to add new role");

        return new Response<string>("User role updated successfully");
    }

    public async Task<Response<string>> LockUnlockUserAsync(string userId, bool lockUser)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        if (lockUser)
            user.LockoutEnd = DateTimeOffset.MaxValue;
        else
            user.LockoutEnd = null;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to update user lock status");

        return new Response<string>($"User {(lockUser ? "locked" : "unlocked")} successfully");
    }

    public async Task<Response<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        if (string.IsNullOrEmpty(currentPassword))
            return new Response<string>(HttpStatusCode.BadRequest, "Current password is required");

        var changeResult = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!changeResult.Succeeded)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to change password");

        return new Response<string>("Password changed successfully");
    }


    public async Task<Response<string>> UpdateUserPhotoAsync(string userId, string? newPhotoUrl)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        user.ImageUrl = newPhotoUrl ?? "";

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to update user photo");

        return new Response<string>("User photo updated successfully");
    }

    public async Task<Response<bool>> IsUsernameTakenAsync(string username)
    {
        logger.LogInformation("IsUsernameTakenAsync called with username={Username}", username);

        var exists = await userManager.FindByNameAsync(username);
        return new Response<bool>(exists != null);
    }
}
