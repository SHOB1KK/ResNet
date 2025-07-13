using System.ComponentModel.DataAnnotations;

public class CreateTableDto
{
    [Required]
    public int RestaurantId { get; set; }

    [Range(1, 20)]
    public int Seats { get; set; }
}
