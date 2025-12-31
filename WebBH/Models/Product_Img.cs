namespace WebBH.Models
{
    public class Product_Img
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsPrimary { get; set; }

        // Navigation Property
        public Product? Product { get; set; }
        //public CartItem? CartItems { get; set; }
    }
}
