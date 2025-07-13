using System.ComponentModel.DataAnnotations;

public class CreateJobApplicationDto
{
    [Required, MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string LastName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string? RestaurantName { get; set; }

    [Required, MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MaxLength(20)]
    [Phone]
    public string Phone { get; set; } = null!;

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
}
