public class ActionLogFilter
{
    public string? UserId { get; set; }

    public string? ActionType { get; set; }

    public string? Entity { get; set; }

    public int? EntityId { get; set; }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}