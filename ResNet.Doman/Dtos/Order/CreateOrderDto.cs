using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace ResNet.Domain.Dtos
{
    public class CreateOrderDto
    {
        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new();

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public int? TableId { get; set; }

        [Required]
        public int RestaurantId { get; set; }


        [Required]
        public string Type { get; set; } = OrderType.Pickup;

        public string? DeliveryAddress { get; set; }

        public DateTime? BookingDateTime { get; set; }

        public string? Status { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}
