namespace ResNet.Domain.Filters;

public class BookingFilter
{
    public string? UserId { get; set; }
    public int? TableId { get; set; }
    public int? RestaurantId { get; set; }
    public DateTime? BookingFrom { get; set; }
    public DateTime? BookingTo { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
