using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Domain.Constants;
using Domain.DTOs.Auth;
using Domain.DTOs.Email;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IConfiguration config;
    private readonly IEmailService emailService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration config,
        IEmailService emailService)
    {
        this.userManager = userManager;
        this.config = config;
        this.emailService = emailService;
    }

    public async Task<Response<TokenDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByNameAsync(loginDto.UserName);
        if (user == null)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        if (!user.EmailConfirmed)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Email is not confirmed");
        }

        var checkPassword = await userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!checkPassword)
        {
            return new Response<TokenDto>(HttpStatusCode.BadRequest, "Username or password is incorrect");
        }

        var token = await GenerateJwt(user);
        return new Response<TokenDto>(new TokenDto { Token = token });
    }


    // public async Task<Response<string>> Register(RegisterDto registerDto)
    // {
    //     var user = new ApplicationUser
    //     {
    //         UserName = registerDto.UserName,
    //         PhoneNumber = registerDto.PhoneNumber,
    //     };

    //     var result = await userManager.CreateAsync(user, registerDto.Password);

    //     if (!result.Succeeded)
    //     {
    //         return new Response<string>(HttpStatusCode.InternalServerError, "Failed to create user");
    //     }

    //     await userManager.AddToRoleAsync(user, Roles.Cashier);
    //     return new Response<string>("User created");
    // }

    public async Task<Response<string>> RequestResetPassword(RequestResetPassword requestResetPassword)
    {
        var user = await userManager.FindByEmailAsync(requestResetPassword.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var emailDto = new EmailDto()
        {
            To = requestResetPassword.Email,
            Subject = "🔒 Восстановление пароля в KASSA POS",
            Body = $"""
    <html>
    <body style="font-family: Arial, sans-serif; background-color: #f7f9fc; color: #333; padding: 20px;">
        <div style="max-width: 600px; margin: auto; background: white; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); padding: 30px;">
            <h2 style="color: #2a9df4; margin-bottom: 10px;">Восстановление пароля</h2>
            <p>Здравствуйте!</p>
            <p>Вы запросили сброс пароля для своего аккаунта KASSA POS.</p>
            <p>Пожалуйста, используйте следующий <strong>токен</strong> для восстановления пароля:</p>
            <div style="background: #e1f0ff; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 2px; border-radius: 6px; margin: 20px 0;">
                {token}
            </div>
            <p>Если вы не запрашивали сброс пароля, просто проигнорируйте это письмо.</p>
            <hr style="border: none; border-top: 1px solid #eee; margin: 30px 0;">
            <p style="font-size: 12px; color: #999;">Это письмо отправлено автоматически, пожалуйста, не отвечайте на него.</p>
            <p style="font-size: 12px; color: #999;">© 2025 KASSA POS. Все права защищены.</p>
        </div>
    </body>
    </html>
    """,
        };

        var result = await emailService.SendEmailAsync(emailDto);

        return !result
            ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send email")
            : new Response<string>("Token sent successfully to email");
    }

    public async Task<Response<string>> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var resetResult =
            await userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);

        return resetResult.Succeeded
            ? new Response<string>("Password reset successfully")
            : new Response<string>(HttpStatusCode.InternalServerError, "Failed to reset password");
    }

    public async Task<Response<string>> SendEmailConfirmationAsync(string userName)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        if (user.EmailConfirmed)
        {
            return new Response<string>(HttpStatusCode.BadRequest, "Email already confirmed");
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        var emailDto = new EmailDto()
        {
            To = user.Email,
            Subject = "Подтверждение Email в ResNet",
            Body = $"""
        <html>
        <body>
        <p>Здравствуйте!</p>
        <p>Для подтверждения вашего Email в ResNet используйте следующий токен:</p>
        <h2>{token}</h2>
        <p>Если вы не запрашивали это письмо, просто проигнорируйте его.</p>
        </body>
        </html>
        """
        };

        var result = await emailService.SendEmailAsync(emailDto);

        return !result
            ? new Response<string>(HttpStatusCode.InternalServerError, "Failed to send confirmation email")
            : new Response<string>("Confirmation email sent successfully");
    }

    public async Task<Response<string>> ConfirmEmailAsync(string userName, string token)
    {
        var user = await userManager.FindByNameAsync(userName);
        if (user == null)
        {
            return new Response<string>(HttpStatusCode.NotFound, "User not found");
        }

        var result = await userManager.ConfirmEmailAsync(user, token);

        if (!result.Succeeded)
        {
            var error = result.Errors.FirstOrDefault()?.Description ?? "Email confirmation failed";
            return new Response<string>(HttpStatusCode.BadRequest, error);
        }

        return new Response<string>("Email confirmed successfully");
    }


    private async Task<string> GenerateJwt(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? "")
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
