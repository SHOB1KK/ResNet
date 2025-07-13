using System.Net;
using System.Threading.Tasks;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Domain.DTOs.Email;
using ResNet.Domain.Entities;

public class NotificationService : INotificationService
{
    private readonly IEmailService emailService;
    private readonly ILogger<NotificationService> logger;

    public NotificationService(IEmailService emailService, ILogger<NotificationService> logger)
    {
        this.emailService = emailService;
        this.logger = logger;
    }

    public async Task SendRestaurantRequestStatusChangedEmail(RestaurantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OwnerEmail))
        {
            logger.LogWarning("RestaurantRequest email is empty. Cannot send status notification.");
            return;
        }

        var subject = request.Status switch
        {
            "Accepted" => "✅ Ваша заявка на добавление ресторана одобрена",
            "Rejected" => "❌ Ваша заявка на добавление ресторана отклонена",
            _ => "ℹ️ Обновление статуса заявки на добавление ресторана"
        };

        var statusMessage = request.Status switch
        {
            "Accepted" => "<p style=\"color:#27ae60; font-weight:600;\">Поздравляем! Ресторан успешно добавлен в систему.</p>",
            "Rejected" => "<p style=\"color:#e74c3c; font-weight:600;\">К сожалению, ваша заявка отклонена.</p>",
            _ => ""
        };

        var body = $"""
        <html>
        <body style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color:#f4f6f8; margin:0; padding:0;">
          <table align="center" width="600" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; box-shadow:0 4px 10px rgba(0,0,0,0.1); margin-top:30px; padding:30px;">
            <tr>
              <td style="text-align:center; padding-bottom:20px;">
                <h1 style="color:#2a9df4; margin-bottom:0;">Статус вашей заявки на добавление ресторана изменён</h1>
                <p style="color:#555555; font-size:16px; margin-top:5px;">Здравствуйте!</p>
              </td>
            </tr>
            <tr>
              <td style="color:#333333; font-size:16px; line-height:1.5; padding-bottom:20px;">
                Ваша заявка на ресторан <strong>{request.Name}</strong> обновлена.<br/>
                <strong>Новый статус:</strong> {request.Status}
                {statusMessage}
              </td>
            </tr>
            <tr>
              <td style="font-size:14px; color:#666666; line-height:1.4; padding-bottom:20px;">
                Спасибо, что пользуетесь нашей платформой!
              </td>
            </tr>
            <tr>
              <td style="text-align:center; font-size:14px; color:#999999;">
                Это автоматическое письмо. Пожалуйста, не отвечайте на него.
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;

        var emailDto = new EmailDto
        {
            To = request.OwnerEmail,
            Subject = subject,
            Body = body
        };

        var sent = await emailService.SendEmailAsync(emailDto);
        if (sent)
            logger.LogInformation("Email sent to {Email} regarding restaurant request status update.", request.OwnerEmail);
        else
            logger.LogWarning("Failed to send email to {Email} regarding restaurant request status update.", request.OwnerEmail);
    }

    public async Task SendJobApplicationStatusChangedEmail(JobApplication application)
    {
        if (string.IsNullOrWhiteSpace(application.Email))
        {
            logger.LogWarning("JobApplication email is empty. Cannot send status notification.");
            return;
        }

        var subject = application.Status switch
        {
            "Accepted" => "✅ Ваша заявка на вакансию принята",
            "Rejected" => "❌ Ваша заявка на вакансию отклонена",
            _ => "ℹ️ Обновление статуса вашей заявки на вакансию"
        };

        var statusMessage = application.Status switch
        {
            "Accepted" => "<p style=\"color:#27ae60; font-weight:600;\">Поздравляем! Мы свяжемся с вами для дальнейших шагов.</p>",
            "Rejected" => "<p style=\"color:#e74c3c; font-weight:600;\">К сожалению, ваша заявка отклонена.</p>",
            _ => ""
        };

        var body = $"""
        <html>
        <body style="font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color:#f4f6f8; margin:0; padding:0;">
          <table align="center" width="600" cellpadding="0" cellspacing="0" style="background-color:#ffffff; border-radius:8px; box-shadow:0 4px 10px rgba(0,0,0,0.1); margin-top:30px; padding:30px;">
            <tr>
              <td style="text-align:center; padding-bottom:20px;">
                <h1 style="color:#2a9df4; margin-bottom:0;">Статус вашей заявки на вакансию изменён</h1>
                <p style="color:#555555; font-size:16px; margin-top:5px;">Здравствуйте, <strong>{application.LastName} {application.FirstName}</strong>!</p>
              </td>
            </tr>
            <tr>
              <td style="color:#333333; font-size:16px; line-height:1.5; padding-bottom:20px;">
                Ваша заявка на позицию <strong>{application.DesiredPosition}</strong> в ресторане <strong>{application.RestaurantName}</strong> обновлена.<br/>
                <strong>Новый статус:</strong> {application.Status}
                {statusMessage}
              </td>
            </tr>
            <tr>
              <td style="font-size:14px; color:#666666; line-height:1.4; padding-bottom:20px;">
                Спасибо за проявленный интерес к работе с нами!
              </td>
            </tr>
            <tr>
              <td style="text-align:center; font-size:14px; color:#999999;">
                Это автоматическое письмо. Пожалуйста, не отвечайте на него.
              </td>
            </tr>
          </table>
        </body>
        </html>
        """;

        var emailDto = new EmailDto
        {
            To = application.Email,
            Subject = subject,
            Body = body
        };

        var sent = await emailService.SendEmailAsync(emailDto);
        if (sent)
            logger.LogInformation("Email sent to {Email} regarding job application status update.", application.Email);
        else
            logger.LogWarning("Failed to send email to {Email} regarding job application status update.", application.Email);
    }

}
