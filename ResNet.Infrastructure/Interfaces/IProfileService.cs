using Domain.Responses;
using Microsoft.AspNetCore.Http;
using ResNet.Domain.Dtos;

public interface IProfileService
{
    Task<Response<GetUserDto>> GetMyProfileAsync(string userId);
    Task<Response<GetUserDto>> UpdateMyProfileAsync(string userId, UpdateUserDto dto);
    Task<Response<string>> ChangePasswordAsync(string userId, ChangePasswordDto dto);
    Task<Response<string>> UploadProfileImageAsync(string userId, IFormFile file);
    Task<Response<string>> DeleteProfileImageAsync(string userId);
}