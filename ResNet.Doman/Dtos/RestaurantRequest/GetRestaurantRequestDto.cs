public class GetRestaurantRequestDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Cuisine { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? OwnerFullName { get; set; }
    public string? OwnerEmail { get; set; }
    public string? OwnerPhone { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime CreatedAt { get; set; }
    public List<WorkingHourDto> WorkingHours { get; set; } = new();
}
