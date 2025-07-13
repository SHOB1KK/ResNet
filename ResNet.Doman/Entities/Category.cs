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

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public List<Product> Products { get; set; } = new();

        public List<RestaurantCategory> RestaurantCategories { get; set; } = new List<RestaurantCategory>();
    }
}
