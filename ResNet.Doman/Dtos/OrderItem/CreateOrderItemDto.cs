using System.ComponentModel.DataAnnotations;

namespace ResNet.Domain.Dtos
{
    public class CreateOrderItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        
        public decimal? PriceAtMoment { get; set; }
    }
}
