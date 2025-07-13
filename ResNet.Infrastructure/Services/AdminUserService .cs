using System.Net;
using Microsoft.AspNetCore.Identity;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Domain.DTOs.Email;
using Domain.Constants;
using ResNet.Domain.Dtos;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;

public class AdminUserService(
UserManager<ApplicationUser> userManager,
ILogger<AdminUserService> logger,
IEmailService emailSender
) : IAdminUserService
{
    public async Task<Response<GetUserDto>> AddUserAsync(CreateUserDto dto, string creatorRole)
    {
        logger.LogInformation("AddUserAsync called by {Role}", creatorRole);

        if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            logger.LogWarning("Validation failed: Missing required fields");
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, "Username, Email, and Password are required");
        }

        var existingUser = await userManager.FindByNameAsync(dto.Username);
        if (existingUser != null)
        {
            logger.LogWarning("Username already exists: {Username}", dto.Username);
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, "Username is already taken");
        }

        var existingEmail = await userManager.FindByEmailAsync(dto.Email);
        if (existingEmail != null)
        {
            logger.LogWarning("Email already exists: {Email}", dto.Email);
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, "Email is already in use");
        }

        var user = new ApplicationUser
        {
            UserName = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName,
            PhoneNumber = dto.PhoneNumber,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Description ?? "Unknown error";
            logger.LogError("User creation failed: {Error}", errorMessage);
            return new Response<GetUserDto>(HttpStatusCode.BadRequest, errorMessage);
        }

        var role = string.IsNullOrWhiteSpace(dto.Role) ? Roles.Cashier : dto.Role;
        await userManager.AddToRoleAsync(user, role);

        if (!string.IsNullOrEmpty(user.Email))
        {
            var email = new EmailDto
            {
                To = user.Email,
                Subject = "🧾 Добро пожаловать в KASSA POS — Ваша регистрация подтверждена!",
                Body = $"""
    <html>
    <body style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color:#f4f6f8; margin:0; padding:0;">
      <table align="center" width="600" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; box-shadow:0 4px 10px rgba(0,0,0,0.1); margin-top:30px; padding:30px;">
        <tr>
          <td style="text-align:center; padding-bottom:20px;">
            <h1 style="color:#2a9df4; margin-bottom:0;">Добро пожаловать в <span style="color:#1e56a0;">KASSA POS</span>!</h1>
            <p style="color:#555555; font-size:16px; margin-top:5px;">Здравствуйте, <strong>{user.UserName}</strong>!</p>
          </td>
        </tr>
        <tr>
          <td style="color:#333333; font-size:16px; line-height:1.5; padding-bottom:20px;">
            Вы были успешно зарегистрированы администратором в системе <strong>KASSA POS</strong>.
          </td>
        </tr>
        <tr>
          <td style="background-color:#f0f4f8; padding:20px; border-radius:6px; margin-bottom:30px;">
            <h2 style="color:#2a9df4; margin-top:0;">🔐 Ваши данные для входа</h2>
            <table style="width:100%; font-size:16px; color:#333333;">
              <tr>
                <td style="padding:8px 0;"><strong>Логин:</strong></td>
                <td style="padding:8px 0; text-align:right;">{user.UserName}</td>
              </tr>
              <tr>
                <td style="padding:8px 0;"><strong>Пароль:</strong></td>
                <td style="padding:8px 0; text-align:right;">{dto.Password}</td>
              </tr>
              <tr>
                <td style="padding:8px 0;"><strong>Роль:</strong></td>
                <td style="padding:8px 0; text-align:right;">{role}</td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td style="font-size:14px; color:#666666; line-height:1.4; padding-bottom:20px;">
            📬 Пожалуйста, не передавайте эти данные другим лицам.<br/>
            🔑 После первого входа обязательно измените пароль на собственный для безопасности.
          </td>
        </tr>
        <tr>
          <td style="text-align:center; font-size:14px; color:#999999;">
            С уважением,<br/>
            <strong>Команда KASSA POS</strong><br/>
          </td>
        </tr>
      </table>
    </body>
    </html>
    """
            };

            await emailSender.SendEmailAsync(email);
        }

        var userDto = new GetUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            ImageUrl = user.ImageUrl,
            Email = user.Email,
            Role = role,
            CreatedAt = user.CreatedAt,
        };

        logger.LogInformation("User {Username} created successfully with role {Role}", user.UserName, role);
        return Response<GetUserDto>.Success(userDto);
    }
}