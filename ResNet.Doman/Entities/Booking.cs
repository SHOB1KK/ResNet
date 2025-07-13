using System;
using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        public int TableId { get; set; }
        public Table Table { get; set; } = null!;

        [Required]
        public DateTime BookingFrom { get; set; }

        [Required]
        public DateTime BookingTo { get; set; }

        [Range(1, 20)]
        public int Guests { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = BookingStatus.Pending;

        [Required, MaxLength(10)]
        public string BookingCode { get; set; } = null!;
    }
}
