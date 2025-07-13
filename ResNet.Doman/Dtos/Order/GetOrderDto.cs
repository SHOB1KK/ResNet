using System.Collections.Generic;

namespace ResNet.Domain.Dtos
{
    public class GetOrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<GetOrderItemDto> Items { get; set; } = new();

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
