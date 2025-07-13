using Domain.Constants;
using Domain.DTOs.Auth;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(IAuthService authService, IAdminUserService adminUserService) : ControllerBase
{
    [HttpPost("add-user")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetUserDto>> AddUser(CreateUserDto createUserDto)
    {
        var creatorRole = User.IsInRole(Roles.Admin) ? Roles.Admin : "Unknown";
        return await adminUserService.AddUserAsync(createUserDto, creatorRole);
    }

    [HttpPost("login")]
    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        return await authService.Login(loginDto);
    }

    [HttpPost("request-reset-password")]
    public async Task<Response<string>> RequestResetPassword(RequestResetPassword request)
    {
        return await authService.RequestResetPassword(request);
    }

    [HttpPost("reset-password")]
    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        return await authService.ResetPassword(resetPasswordDto);
    }

    [HttpPost("send-confirmation-email")]
    public async Task<Response<string>> SendEmailConfirmation([FromQuery] string userName)
    {
        return await authService.SendEmailConfirmationAsync(userName);
    }

    [HttpPost("confirm-email")]
    public async Task<Response<string>> ConfirmEmail([FromQuery] string userName, [FromQuery] string token)
    {
        return await authService.ConfirmEmailAsync(userName, token);
    }
}
