using System;

namespace ResNet.Domain.Dtos
{
    public class GetBookingDto
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public int TableId { get; set; }
        public DateTime BookingTime { get; set; }
        public int Guests { get; set; }
        public string Status { get; set; } = null!;
    }
}
