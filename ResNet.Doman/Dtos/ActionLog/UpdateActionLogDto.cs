using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos;

public class UpdateActionLogDto
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ActionType { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Entity { get; set; } = null!;

    public int EntityId { get; set; }
}
