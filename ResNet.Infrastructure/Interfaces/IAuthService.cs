using Domain.DTOs.Auth;
using Domain.Responses;
using ResNet.Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IAuthService
{
    Task<Response<TokenDto>> Login(LoginDto loginDto);
    // Task<Response<string>> Register(RegisterDto registerDto);
    Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword);
    Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto);
    Task<Response<string>> SendEmailConfirmationAsync(string userName);
    Task<Response<string>> ConfirmEmailAsync(string userName, string token);
}
