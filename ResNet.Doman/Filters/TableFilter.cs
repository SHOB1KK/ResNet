namespace ResNet.Domain.Filters;

public class TableFilter
{
    public int? RestaurantId { get; set; }
    public int? MinSeats { get; set; }
    public int? MaxSeats { get; set; }
    public string? Status { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
