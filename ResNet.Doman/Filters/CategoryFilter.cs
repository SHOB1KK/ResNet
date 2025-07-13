namespace ResNet.Domain.Filters;

public class CategoryFilter
{
    public string? Name { get; set; }
    public int? RestaurantId { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
