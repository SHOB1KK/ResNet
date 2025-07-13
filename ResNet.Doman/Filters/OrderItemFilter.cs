public class OrderItemFilter
{
    public int? OrderId { get; set; }

    public int? ProductId { get; set; }

    public int? QuantityFrom { get; set; }

    public int? QuantityTo { get; set; }
    
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}