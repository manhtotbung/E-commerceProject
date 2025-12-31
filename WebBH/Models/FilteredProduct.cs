namespace WebBH.Models
{
    public class FilteredProduct
    {
        public string? SearchTerm { get; set; }
        public List<int>? CategoryId { get; set; }
        public List<string>? Size { get; set; }
        public List<String>? PriceRange { get; set; }
        public List<String>? Color { get; set; }
    }
}
