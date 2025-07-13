using System;
using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class CreateBookingDto
    {
        [Required]
        public int TableId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public DateTime BookingFrom { get; set; }

        [Required]
        public DateTime BookingTo { get; set; }

        [Range(1, 20)]
        public int Guests { get; set; }
    }
}
