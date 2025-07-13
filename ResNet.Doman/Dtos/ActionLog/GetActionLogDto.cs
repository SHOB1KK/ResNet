namespace ResNet.Domain.Dtos;

public class GetActionLogDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string ActionType { get; set; } = null!;
    public string Entity { get; set; } = null!;
    public int EntityId { get; set; }
    public DateTime Timestamp { get; set; }
}
