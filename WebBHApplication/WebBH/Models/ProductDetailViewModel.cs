using System.ComponentModel.DataAnnotations;

namespace WebBH.Models
{
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProduct { get; set; }

    }
}
