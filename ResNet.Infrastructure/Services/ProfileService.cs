using System.Net;
using AutoMapper;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;

public class ProfileService(
    UserManager<ApplicationUser> userManager,
    IMapper mapper,
    IFileService fileService,
    ILogger<ProfileService> logger
) : IProfileService
{
    public async Task<Response<GetUserDto>> GetMyProfileAsync(string userId)
    {
        logger.LogInformation("GetMyProfileAsync called for userId={UserId}", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User with id={UserId} not found", userId);
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "User not found");
        }

        var roles = await userManager.GetRolesAsync(user);
        var dto = new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName ?? "",
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            ImageUrl = user.ImageUrl ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            Role = roles.FirstOrDefault() ?? "",
            CreatedAt = user.LockoutEnd?.DateTime ?? DateTime.UtcNow,
            RestaurantName = user.Restaurant?.Name
        };

        return Response<GetUserDto>.Success(dto);
    }

    public async Task<Response<GetUserDto>> UpdateMyProfileAsync(string userId, UpdateUserDto dto)
    {
        logger.LogInformation("UpdateMyProfileAsync called for userId={UserId}", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User with id={UserId} not found", userId);
            return new Response<GetUserDto>(HttpStatusCode.NotFound, "User not found");
        }

        user.UserName = dto.Username;
        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.PhoneNumber = dto.PhoneNumber;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Failed to update profile";
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, error);
        }

        var roles = await userManager.GetRolesAsync(user);
        var updatedDto = new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName ?? "",
            FullName = user.FullName ?? "",
            Email = user.Email ?? "",
            ImageUrl = user.ImageUrl ?? "",
            PhoneNumber = user.PhoneNumber ?? "",
            Role = roles.FirstOrDefault() ?? "",
            CreatedAt = user.LockoutEnd?.DateTime ?? DateTime.UtcNow
        };

        return Response<GetUserDto>.Success(updatedDto);
    }

    public async Task<Response<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto)
    {
        logger.LogInformation("ChangePasswordAsync called for userId={UserId}", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("User with id={UserId} not found", userId);
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Failed to change password";
            return new Response<string>(HttpStatusCode.BadRequest, error);
        }

        return Response<string>.Success("Password changed successfully");
    }

    public async Task<Response<string>> UploadProfileImageAsync(string userId, IFormFile file)
    {
        logger.LogInformation("UploadProfileImageAsync called for userId={UserId}", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        if (file == null || file.Length == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "File is empty");

        var imageUrl = await fileService.UploadFileAsync(file, "profiles");

        if (!string.IsNullOrWhiteSpace(user.ImageUrl))
            await fileService.DeleteFileAsync(user.ImageUrl);

        user.ImageUrl = imageUrl;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Failed to update profile image";
            return new Response<string>(HttpStatusCode.BadRequest, error);
        }

        return Response<string>.Success(imageUrl);
    }


    public async Task<Response<string>> DeleteProfileImageAsync(string userId)
    {
        logger.LogInformation("DeleteProfileImageAsync called for userId={UserId}", userId);

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return new Response<string>(HttpStatusCode.NotFound, "User not found");

        if (string.IsNullOrWhiteSpace(user.ImageUrl))
            return new Response<string>(HttpStatusCode.BadRequest, "No profile image to delete");

        var deleted = await fileService.DeleteFileAsync(user.ImageUrl);
        if (!deleted)
            return new Response<string>(HttpStatusCode.BadRequest, "Failed to delete image file");

        user.ImageUrl = null;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Failed to delete profile image";
            return new Response<string>(HttpStatusCode.BadRequest, error);
        }

        return Response<string>.Success("Profile image deleted successfully");
    }


}
