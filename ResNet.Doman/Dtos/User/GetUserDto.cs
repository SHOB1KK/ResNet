using System;

namespace ResNet.Domain.Dtos
{
    public class GetUserDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Role { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? RestaurantName { get; set; } 
    }
}
