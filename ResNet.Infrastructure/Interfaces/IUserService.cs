using Domain.Responses;
using ResNet.Domain.Dtos;

namespace Infrastructure.Interfaces;

public interface IUserService
{
    Task<PagedResponse<List<GetUserDto>>> GetAllUsersAsync(UserFilter filter);
    Task<Response<GetUserDto>> GetUserByIdAsync(string id);
    Task<Response<GetUserDto>> UpdateUserAsync(string id, UpdateUserDto userDto);
    Task<Response<string>> DeleteUserAsync(string id);
    Task<Response<string>> ChangeUserRoleAsync(string userId, string newRole);
    Task<Response<string>> LockUnlockUserAsync(string userId, bool lockUser);
    Task<Response<string>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    Task<Response<string>> UpdateUserPhotoAsync(string userId, string? newPhotoUrl);
    Task<Response<bool>> IsUsernameTakenAsync(string username);
}