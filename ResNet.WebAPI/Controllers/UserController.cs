using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class UserController(IUserService userService)
{
    [HttpGet]
    public async Task<PagedResponse<List<GetUserDto>>> GetAllUsers([FromQuery] UserFilter filter)
    {
        return await userService.GetAllUsersAsync(filter);
    }

    [HttpGet("{id}")]
    public async Task<Response<GetUserDto>> GetUserById(string id)
    {
        return await userService.GetUserByIdAsync(id);
    }

    [HttpPut("{id}")]
    public async Task<Response<GetUserDto>> UpdateUser(string id, UpdateUserDto request)
    {
        return await userService.UpdateUserAsync(id, request);
    }

    [HttpDelete("{id}")]
    public async Task<Response<string>> DeleteUser(string id)
    {
        return await userService.DeleteUserAsync(id);
    }

    [HttpPut("{id}/role")]
    public async Task<Response<string>> ChangeUserRole(string id, [FromBody] string newRole)
    {
        return await userService.ChangeUserRoleAsync(id, newRole);
    }

    [HttpPut("{id}/lock")]
    public async Task<Response<string>> LockUnlockUser(string id, [FromQuery] bool lockUser)
    {
        return await userService.LockUnlockUserAsync(id, lockUser);
    }

    [HttpPut("{id}/password")]
    public async Task<Response<string>> ChangePassword(string id, [FromBody] ChangePasswordDto dto)
    {
        return await userService.ChangePasswordAsync(id, dto.CurrentPassword, dto.NewPassword);
    }

    [HttpPost("{id:int}/photo")]
    [RequestSizeLimit(10_000_000)]
    [Consumes("multipart/form-data")]
    public async Task<Response<string>> UploadUserPhoto([FromRoute] string id, [FromForm] UploadImageDto dto)
    {
        if (dto?.Image == null || dto.Image.Length == 0)
            return new Response<string>(System.Net.HttpStatusCode.BadRequest, "File is required");

        return await userService.UploadUserImageAsync(id, dto.Image);
    }

    [HttpDelete("{id}/photo")]
    public async Task<Response<string>> DeleteUserPhoto([FromRoute] string id)
    {
        return await userService.DeleteUserImageAsync(id);
    }

    [HttpGet("check-username")]
    public async Task<Response<bool>> IsUsernameTaken([FromQuery] string username)
    {
        return await userService.IsUsernameTakenAsync(username);
    }
}
