using System;

namespace ResNet.Domain.Dtos
{
    public class GetProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        
        public int RestaurantId { get; set; }

        public string CategoryName { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
    }
}
