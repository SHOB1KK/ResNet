namespace ResNet.Domain.Filters;

public class BookingFilter
{
    public string? UserId { get; set; }
    public int? TableId { get; set; }
    public int? RestaurantId { get; set; }
    public DateTime? Date { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
