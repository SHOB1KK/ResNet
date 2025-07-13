using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Constants;
using ResNet.Domain.Entities;

public class Table
{
    public int Id { get; set; }

    [Required]
    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;

    [Range(1, 20)]
    public int Seats { get; set; }

    [Required, MaxLength(20)]
    public string Status { get; set; } = TableStatus.Available;
}
