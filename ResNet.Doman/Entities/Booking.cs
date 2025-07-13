using System;
using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Required]
        public int TableId { get; set; }
        public Table Table { get; set; } = null!;

        [Required]
        public DateTime BookingTime { get; set; }

        [Range(1, 20)]
        public int Guests { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = BookingStatus.Pending;
    }
}
