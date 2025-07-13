public class GetTableDto
{
    public int Id { get; set; }
    public int RestaurantId { get; set; }
    public int Seats { get; set; }
    public string Status { get; set; } = null!;
}
