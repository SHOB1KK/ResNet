namespace ResNet.Domain.Dtos
{
    public class GetOrderItemDto
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtMoment { get; set; }

        public string? ProductName { get; set; }
    }
}
