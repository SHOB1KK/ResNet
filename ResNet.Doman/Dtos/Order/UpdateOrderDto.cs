using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class UpdateOrderDto
    {
        public int Id { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }

        public string? Status { get; set; }
        public decimal? TotalAmount { get; set; }
    }
}
