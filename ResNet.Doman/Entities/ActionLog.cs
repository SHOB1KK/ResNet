using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Entities;

public class ActionLog
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;

    [Required, MaxLength(100)]
    public string ActionType { get; set; } = null!;

    [Required, MaxLength(100)]
    public string Entity { get; set; } = null!;

    public int EntityId { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
