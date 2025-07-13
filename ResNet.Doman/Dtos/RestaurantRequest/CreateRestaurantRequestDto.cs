using System.ComponentModel.DataAnnotations;

public class CreateRestaurantRequestDto
{
    [Required, MaxLength(150)]
    public string Name { get; set; } = null!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Cuisine { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? OwnerFullName { get; set; }

    [MaxLength(100)]
    [EmailAddress]
    public string? OwnerEmail { get; set; }

    [MaxLength(20)]
    [Phone]
    public string? OwnerPhone { get; set; }
}
