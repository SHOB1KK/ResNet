using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Entities;

public class UpdateTableDto
{
    [Range(1, 20)]
    public int Seats { get; set; }

    [Required]
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    [Required, MaxLength(20)]
    public string Status { get; set; } = null!;
}
