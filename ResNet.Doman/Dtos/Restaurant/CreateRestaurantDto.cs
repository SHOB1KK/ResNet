using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class CreateRestaurantDto
    {
        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? Cuisine { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; } = 0;

        public string? OpeningHours { get; set; }
    }
}
