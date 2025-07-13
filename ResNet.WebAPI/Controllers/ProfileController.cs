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

    [HttpPost("me/photo")]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<Response<string>> UploadRestaurantPhoto([FromForm] UploadImageDto dto)
    {
        if (dto?.Image == null || dto.Image.Length == 0)
            return new Response<string>(System.Net.HttpStatusCode.BadRequest, "File is required");

        var userId = GetUserId();
        return await profileService.UploadProfileImageAsync(userId, dto.Image);
    }

    [HttpDelete("me/photo")]
    public async Task<Response<string>> DeleteProfilePhoto()
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
