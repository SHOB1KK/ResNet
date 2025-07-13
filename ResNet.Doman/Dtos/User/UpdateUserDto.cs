using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Constants;

namespace ResNet.Domain.Dtos
{
    public class UpdateUserDto
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        [Required, Phone, MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public string Role { get; set; } = Roles.Cashier;
    }
}
