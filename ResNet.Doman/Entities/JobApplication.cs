using System.ComponentModel.DataAnnotations;
using Domain.Constants;

public class JobApplication
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required, MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MaxLength(20)]
    [Phone]
    public string Phone { get; set; } = null!;

    [Required, MaxLength(200)]
    public string RestaurantName { get; set; } = null!;

    [MaxLength(200)]
    public string? Address { get; set; }

    [Required, MaxLength(100)]
    public string DesiredPosition { get; set; } = null!;

    [MaxLength(1000)]
    public string? WorkExperience { get; set; }

    [MaxLength(1000)]
    public string? MotivationLetter { get; set; }

    [MaxLength(300)]
    public string? ResumeUrl { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(20)]
    public string Status { get; set; } = RequestStatus.Pending;
}
