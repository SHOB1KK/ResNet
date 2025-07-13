using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; } = null!;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public List<OrderItem> OrderItems { get; set; } = new();
    }
}
