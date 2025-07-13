using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
