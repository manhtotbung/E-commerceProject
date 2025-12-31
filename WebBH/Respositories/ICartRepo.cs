using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WebBH.Models;

namespace WebBH.Respositories   
{
    public interface ICartRepo
    {
        // Lấy giỏ từ session (chưa đăng nhập)
        List<CartItem> GetSessionCart(HttpContext http);
        void SaveSessionCart(HttpContext http, List<CartItem> items);  // Lưu giỏ vào session
        Task<List<Cart>> GetDbCartAsync(string userId);      // Lấy giỏ từ DB (đã đăng nhập)
        Task MergeSessionToDbAsync(HttpContext http, string userId);   // Gộp session -> DB khi login


        Task<ProductVariant?> ResolveVariantAsync(int productId, int? variantId); // Biến thể: lấy đúng variant (nếu null thì lấy default theo IsDefault)
        // Upsert is insert + update 
        Task UpsertDbAsync(string userId, int productId, int variantId, int quantity);
        Task UpsertSessionAsync(HttpContext http, int productId, int variantId, int quantity);

        //remove cart
        Task RemoveDbCartAsync(string userId, int productId, int? variantId);
        void RemoveSessionCart(HttpContext http, int productId, int? variantId);

        //order 
        //Task CheckOrderItem();
        Task<int> CreateOrderFromDbCartAsync(string userId, CheckOutVM model);
        Task<Order?> GetOrderWithItemsAsync(int orderId, string? userId);

    }
}
