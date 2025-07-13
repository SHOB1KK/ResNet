public class GetJobApplicationDto
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Address { get; set; }

    public string DesiredPosition { get; set; } = null!;

    public string? WorkExperience { get; set; }

    public string? MotivationLetter { get; set; }

    public string? ResumeUrl { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }
}
