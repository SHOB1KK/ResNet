using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class UpdateBookingDto
    {
        [Required]
        public DateTime BookingTime { get; set; }

        [Range(1, 20)]
        public int Guests { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = null!;
    }
}
