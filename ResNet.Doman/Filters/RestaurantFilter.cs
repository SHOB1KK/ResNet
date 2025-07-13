public class RestaurantFilter
{
    public string? Name { get; set; }
    public string? Cuisine { get; set; }
    public double? MinRating { get; set; }
    public double? MaxRating { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}