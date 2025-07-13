using Domain.Responses;
using ResNet.Domain.Dtos;

namespace Infrastructure.Interfaces;

public interface IAdminUserService
{
    Task<Response<GetUserDto>> AddUserAsync(CreateUserDto userDto, string role);
}