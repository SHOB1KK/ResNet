using Domain.DTOs.Email;
using Domain.Responses;

namespace Infrastructure.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(EmailDto emailDto);
}
