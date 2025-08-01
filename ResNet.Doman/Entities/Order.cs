using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Constants;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Type { get; set; } = OrderType.Pickup;

        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? DeliveryAddress { get; set; }

        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        public int? TableId { get; set; }
        public Table? Table { get; set; }

    }
}
