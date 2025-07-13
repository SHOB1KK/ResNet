using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Entities
{
    public class Restaurant
    {
        public int Id { get; set; }

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

        public string? OpeningHours { get; set; }

        [Range(0, 5)]
        public double Rating { get; set; } = 0;

        public List<Table> Tables { get; set; } = new();

        public List<Category> Categories { get; set; } = new();

        public List<Product> Menu { get; set; } = new();
    }
}
