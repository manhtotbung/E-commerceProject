using WebBH.Models;

namespace WebBH
{
    public interface IProductRepo
    {
        Task<List<Product>> GetFilteredProductAsync(FilteredProduct filteredProduct);       
        Task<ProductDetailViewModel> GetProductByIdAsync(int id);
    }
}