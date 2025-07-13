using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class UpdateCategoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? ImageUrl { get; set; }
    }
}
