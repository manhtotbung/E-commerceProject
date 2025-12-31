using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebBH.Models
{
    public class Cart
    {
        public int Id { get; set; }

        [Required] 
        public string UserId { get; set; } = default!;

        // Sản phẩm & biến thể
        public int ProductId { get; set; } 
        public int? VariantId { get; set; } 

        public int Quantity { get; set; }

        [Precision(18, 2)]
        public decimal UnitPrice { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;


        public Product? Product { get; set; }
        public ProductVariant? Variant { get; set; }

    }
   
}
