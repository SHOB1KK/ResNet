namespace ResNet.Domain.Filters;

public class ApplicationFilter
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RestaurantName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? DesiredPosition { get; set; }
    public string? Status { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
