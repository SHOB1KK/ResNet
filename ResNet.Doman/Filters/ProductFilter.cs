namespace ResNet.Domain.Filters;

public class ProductFilter
{
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
    public int? RestaurantId { get; set; }

    public decimal? PriceFrom { get; set; }
    public decimal? PriceTo { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
