using System.ComponentModel.DataAnnotations;

namespace WebBH.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        //đây là quan hệ 1-n, navigate khóa ngoại cho  product variant
        public List<ProductVariant> Variants { get; set; }
        public Category Categories { get; set; }
        public List<Product_Img> Product_Imgs { get; set; } 
        public List<Cart> Carts { get; set; }

    }
 
}
