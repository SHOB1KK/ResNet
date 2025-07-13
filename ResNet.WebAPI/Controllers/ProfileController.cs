using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Dtos;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

    [HttpGet("me")]
    public async Task<Response<GetUserDto>> GetMyProfile()
    {
        var userId = GetUserId();
        return await profileService.GetMyProfileAsync(userId);
    }

    [HttpPut("me")]
    public async Task<Response<GetUserDto>> UpdateMyProfile(UpdateUserDto dto)
    {
        var userId = GetUserId();
        return await profileService.UpdateMyProfileAsync(userId, dto);
    }

    [HttpPut("me/image")]
    public async Task<Response<string>> UpdateProfileImage([FromBody] string imageUrl)
    {
        var userId = GetUserId();
        return await profileService.UpdateProfileImageAsync(userId, imageUrl);
    }

    [HttpDelete("me/image")]
    public async Task<Response<string>> DeleteProfileImage()
    {
        var userId = GetUserId();
        return await profileService.DeleteProfileImageAsync(userId);
    }

    [HttpPut("me/password")]
    public async Task<Response<string>> ChangePassword(ChangePasswordDto dto)
    {
        var userId = GetUserId();
        return await profileService.ChangePasswordAsync(userId, dto);
    }
}
