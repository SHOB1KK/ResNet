using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Auth;

public class RegisterDto
{
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
}
