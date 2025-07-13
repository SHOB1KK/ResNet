using System;
using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class CreateBookingDto
    {
        public string? UserId { get; set; }

        [Required]
        public int TableId { get; set; }

        [Required]
        public DateTime BookingTime { get; set; }

        [Range(1, 20)]
        public int Guests { get; set; }
    }
}
