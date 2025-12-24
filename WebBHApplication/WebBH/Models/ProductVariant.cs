using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBH.Models
{
  
    public class ProductVariant
    {
        public int Id { get; set; }
        
        [Required]
        public int ProductId { get; set; }

        [Required]
        public string? ColorName { get; set; }

        [Required]
        public string? ColorId { get; set; }

        [Required]
        public string? Size { get; set; }

        [Required]
        [Precision(18, 2)] //18 la tong chúữ số, 2 là 2 số sau thập phân
        public decimal Price { get; set; }

        [Required]
        public int? Stock { get; set; }
        public bool IsDefault { get; set; }
        //đây là qh 1-1 
        public Product Product { get; set; } 
        public List<Cart> Carts { get; set; }


    }

}
