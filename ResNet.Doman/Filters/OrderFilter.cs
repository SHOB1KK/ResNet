public class OrderFilter
{
    public string? UserId { get; set; }
    
    public string? FullName { get; set; }
    
    public string? PhoneNumber { get; set; }
    
    public decimal? TotalAmountFrom { get; set; }
    public decimal? TotalAmountTo { get; set; }
    
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public string? Status { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
