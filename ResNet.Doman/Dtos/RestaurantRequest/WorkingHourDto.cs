using System.ComponentModel.DataAnnotations;

public class WorkingHourDto
{
    public string DayOfWeek { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public TimeSpan From { get; set; }
    public TimeSpan To { get; set; }
}
