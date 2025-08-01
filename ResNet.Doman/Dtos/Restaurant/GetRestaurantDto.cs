using System.Collections.Generic;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Dtos
{
    public class GetRestaurantDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string? Cuisine { get; set; }

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public double Rating { get; set; }

        public string? ImageUrl { get; set; }

        public string? OpeningHours { get; set; }

        public List<GetCategoryDto>? Categories { get; set; }

        public List<GetProductDto>? Menu { get; set; }

        public List<GetTableDto>? Tables { get; set; }
        public List<ApplicationUser>? Users { get; set; }

    }
}
