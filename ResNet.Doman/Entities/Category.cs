using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public List<Product> Products { get; set; } = new();
    }
}
