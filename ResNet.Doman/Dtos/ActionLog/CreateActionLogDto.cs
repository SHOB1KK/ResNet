using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos;

public class CreateActionLogDto
{
    [Required]
    public string? UserId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ActionType { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Entity { get; set; } = null!;

    public int EntityId { get; set; }
}
