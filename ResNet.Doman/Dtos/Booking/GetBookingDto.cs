using System;

namespace ResNet.Domain.Dtos
{
    public class GetBookingDto
    {
        public int Id { get; set; }
        public int TableId { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime BookingFrom { get; set; }
        public DateTime BookingTo { get; set; }
        public int Guests { get; set; }
        public string Status { get; set; } = null!;
    }
}
