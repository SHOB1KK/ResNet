public class UserFilter
{
    public string? Username { get; set; }

    public string? Role { get; set; }

    public DateTime? CreatedFrom { get; set; }

    public DateTime? CreatedTo { get; set; }
    
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}