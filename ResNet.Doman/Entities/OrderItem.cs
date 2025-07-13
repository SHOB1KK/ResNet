using System.ComponentModel.DataAnnotations;
using ResNet.Domain.Entities;

namespace ResNet.Domain.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal PriceAtMoment { get; set; }
    }
}
