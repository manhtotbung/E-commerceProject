using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using WebBH.Data;
using WebBH.Extensions;
using WebBH.Models;
using static System.Net.WebRequestMethods;

namespace WebBH.Respositories
{
    public class CartRepo: ICartRepo
    {
        private readonly ApplicationDbContext _context;
       
        public CartRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CartItem> GetSessionCart(HttpContext http) =>
            http.Session.GetObject<List<CartItem>>(MySetting.CART_KEY) ?? new();

        public void SaveSessionCart(HttpContext http, List<CartItem> items) =>
            http.Session.SetObject(MySetting.CART_KEY, items);

        public async Task<List<Cart>> GetDbCartAsync(string userId)
        {
            return await _context.Carts 
                .Include(c => c.Product)
                  .ThenInclude(p => p.Product_Imgs)
                .Include(c => c.Variant)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task MergeSessionToDbAsync(HttpContext http, string userId)
        {
            var sess = GetSessionCart(http);
            if (sess.Count == 0) return;

            var dbItems = await _context.Carts
                .Where(x => x.UserId == userId)
                .ToListAsync();

            foreach (var s in sess)
            {
                // s.VariantId đã nằm trong session CartItem 
                var exist = dbItems.FirstOrDefault(d =>
                    d.ProductId == s.ProductId && d.VariantId == s.VariantId);

                if (exist != null)
                {
                    exist.Quantity += s.Quantity;
                }
                else
                {
                    _context.Carts.Add(new Cart
                    {
                        UserId = userId,
                        ProductId = s.ProductId,
                        VariantId = s.VariantId, 
                        Quantity = s.Quantity,
                        UnitPrice = s.UnitPrice
                    });
                }
            }

            await _context.SaveChangesAsync();
            http.Session.Remove(MySetting.CART_KEY); 
        }

        public async Task<ProductVariant?> ResolveVariantAsync(int productId, int? variantId)
        {
            if (variantId.HasValue)
                return await _context.ProductVariants
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == variantId.Value && x.ProductId == productId);

            return await _context.ProductVariants
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.Id)
                .FirstOrDefaultAsync();
        }

        public async Task UpsertDbAsync(string userId, int productId, int variantId, int quantity)
        {
            var price = await _context.ProductVariants
                            .Where(v => v.Id == variantId && v.ProductId == productId)
                            .Select(v => v.Price)
                            .SingleAsync(); // đảm bảo hợp lệ

            var line = await _context.Carts.FirstOrDefaultAsync(c =>
                c.UserId == userId && c.ProductId == productId && c.VariantId == variantId);

            if (line != null) line.Quantity += quantity;
            else
                _context.Carts.Add(new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    VariantId = variantId,
                    Quantity = quantity,
                    UnitPrice = price
                });

            await _context.SaveChangesAsync();
        }

        public async Task UpsertSessionAsync(HttpContext http, int productId, int variantId, int quantity)
        {
            var cart = GetSessionCart(http);

            var v = await _context.ProductVariants
                    .Include(x => x.Product).ThenInclude(p => p.Product_Imgs)
                    .SingleAsync(x => x.Id == variantId && x.ProductId == productId);

            var item = cart.SingleOrDefault(x => x.ProductId == productId && x.VariantId == variantId);
            if (item == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    VariantId = variantId,
                    ProductName = v.Product.Name,
                    ProductImage = v.Product.Product_Imgs.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                                   ?? v.Product.Product_Imgs.FirstOrDefault()?.ImageUrl
                                   ?? "default-image-url",
                    Quantity = quantity,
                    UnitPrice = v.Price,
                    Color = v.ColorName,
                    Size = v.Size
                });
            }
            else item.Quantity += quantity;

            http.Session.SetObject(MySetting.CART_KEY, cart);
        }
        public async Task RemoveDbCartAsync(string userId, int productId, int? variantId)
        {
                var line = await _context.Carts.FirstOrDefaultAsync(c =>
                  c.UserId == userId &&
                  c.ProductId == productId &&
                  c.VariantId == variantId);

                if (line != null)
                {
                    _context.Carts.Remove(line);
                    await _context.SaveChangesAsync();
                }

        }

        public void RemoveSessionCart(HttpContext http, int productId, int? variantId)
        {
            var cart = GetSessionCart(http);
            var item = cart.SingleOrDefault(p => p.ProductId == productId
                                              && (variantId == null || p.VariantId == variantId));
            if (item != null)
            {
                cart.Remove(item);
                SaveSessionCart(http, cart);
            }
        }


        public async Task<int> CreateOrderFromDbCartAsync(string userId, CheckOutVM model)
        {
            var dbCart = await _context.Carts
                            .Include(c => c.Product).ThenInclude(p => p.Product_Imgs)
                            .Include(c => c.Variant)
                            .Where(c => c.UserId == userId)
                            .ToListAsync();

            if (dbCart.Count == 0) throw new InvalidOperationException("Cart is empty.");
            var order = new Order
            {
                UserId = userId,
                FullName = model.FullName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                PaymentMethod = model.PaymentMethod ?? "COD",
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                Note = model.Note ?? string.Empty 
            };


            foreach (var line in dbCart)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = line.ProductId,
                    VariantId = line.VariantId,
                    ProductName = line.Product!.Name,
                    Color = line.Variant?.ColorName,
                    Size = line.Variant?.Size,
                    UnitPrice = line.UnitPrice,
                    Quantity = line.Quantity
                });
            }

            order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            order.ShippingFee = 0;
            order.Discount = 0;

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _context.Carts.RemoveRange(dbCart);
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return order.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<Order?> GetOrderWithItemsAsync(int orderId, string? userId)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .AsQueryable();

            // Nếu truyền userId, chỉ trả đơn của chính user đó (hoặc admin kiểm tra khác)
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.UserId == userId);
            }

            return await query.FirstOrDefaultAsync(o => o.Id == orderId);
        }

    }
}
