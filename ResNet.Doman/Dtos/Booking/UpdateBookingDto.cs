using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class UpdateBookingDto
    {
        [Required]
        public DateTime BookingFrom { get; set; }

        [Required]
        public DateTime BookingTo { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [Range(1, 20)]
        public int Guests { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = null!;
    }
}
