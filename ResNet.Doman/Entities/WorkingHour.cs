public class WorkingHour
{
    public int Id { get; set; }
    public DayOfWeek Day { get; set; }
    public TimeSpan OpenTime { get; set; }
    public TimeSpan CloseTime { get; set; }

    public int RestaurantRequestId { get; set; }
    public RestaurantRequest RestaurantRequest { get; set; } = null!;
}
