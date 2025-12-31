using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBH.Areas.Data;
using WebBH.Data;
using WebBH.Extensions;
using WebBH.Models;
using WebBH.Respositories;

namespace WebBH.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartRepo _cartRepo;
        private readonly UserManager<AppUser> _userManager;
        public CartController( ICartRepo cartRepo, UserManager<AppUser> userManager) {
            
            _cartRepo = cartRepo;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;
                var dbCart = await _cartRepo.GetDbCartAsync(userId);
        
                var viewModel = dbCart.Select(x => new CartItem
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    ProductName = x.Product?.Name,
                    ProductImage =
                    x.Product?.Product_Imgs?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
                    ?? x.Product?.Product_Imgs?.FirstOrDefault()?.ImageUrl
                    ?? "default-image-url",
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Color = x.Variant?.ColorName,
                    Size = x.Variant?.Size
                }).ToList();

                return View(viewModel);
            }
            else
            {
                return View(_cartRepo.GetSessionCart(HttpContext));
            }
        }

        public async Task<IActionResult> AddToCart(int id, int? variantId, int quantity = 1)
        {
            // 1) Xác định biến thể: nếu null → lấy default theo IsDefault
            var v = await _cartRepo.ResolveVariantAsync(id, variantId);
            if (v == null)
                return BadRequest("Vui lòng chọn biến thể (màu/size).");

            // 2) Upsert theo nơi lưu
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;
                await _cartRepo.UpsertDbAsync(userId, id, v.Id, quantity);
            }
            else
            {
                await _cartRepo.UpsertSessionAsync(HttpContext, id, v.Id, quantity);
            }

            return RedirectToAction(nameof(Index));
        }

       
        public async Task<IActionResult> RemoveCart(int id, int? variantId = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = _userManager.GetUserId(User)!;
                await _cartRepo.RemoveDbCartAsync(userId, id, variantId);
            }
            else
            {
                _cartRepo.RemoveSessionCart(HttpContext, id, variantId);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> CheckOut()
        {
            var userId = _userManager.GetUserId(User)!;

            // sau khi login xong, merge giỏ session -> DB
            await _cartRepo.MergeSessionToDbAsync(HttpContext, userId); // đảm bảo DB-cart có hàng
            var dbCart = await _cartRepo.GetDbCartAsync(userId);

            if (dbCart.Count == 0) return RedirectToAction("Index", "Cart");

            var items = dbCart.Select(x => new CartItem
            {
                ProductId = x.ProductId,
                VariantId = x.VariantId,
                ProductName = x.Product?.Name,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                Color = x.Variant?.ColorName,
                Size = x.Variant?.Size
            }).ToList();

            return View(new CheckOutVM { Items = items });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckOutVM model)
        {
            var userId = _userManager.GetUserId(User)!;

            // NEW: hợp nhất giỏ session vào DB để chắc chắn DB-cart không rỗng
            await _cartRepo.MergeSessionToDbAsync(HttpContext, userId); // ✅:contentReference[oaicite:4]{index=4}

            // Lấy lại DB-cart sau khi merge
            var dbCart = await _cartRepo.GetDbCartAsync(userId); // ✅ Include sẵn Product/Variant:contentReference[oaicite:5]{index=5}
            if (dbCart.Count == 0)
                ModelState.AddModelError(string.Empty, "Giỏ hàng trống.");

            if (!ModelState.IsValid)
            {
                model.Items = dbCart.Select(x => new CartItem
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    ProductName = x.Product?.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Color = x.Variant?.ColorName,
                    Size = x.Variant?.Size
                }).ToList();
                return View("CheckOut", model);
            }

            int orderId;
            try
            {
                orderId = await _cartRepo.CreateOrderFromDbCartAsync(userId, model); 
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("Index", "Cart");
            }
            catch
            {
                ModelState.AddModelError("", "Không thể tạo đơn hàng. Vui lòng thử lại.");
                model.Items = dbCart.Select(x => new CartItem
                {
                    ProductId = x.ProductId,
                    VariantId = x.VariantId,
                    ProductName = x.Product?.Name,
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice,
                    Color = x.Variant?.ColorName,
                    Size = x.Variant?.Size
                }).ToList();
                return View("CheckOut", model);
            }

            return RedirectToAction("ThankYou", "Cart", new { id = orderId }); // ✅ ThankYou nằm ở CartController:contentReference[oaicite:7]{index=7}
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ThankYou(int id)
        {
            var userId = _userManager.GetUserId(User)!;
            var order = await _cartRepo.GetOrderWithItemsAsync(id, userId);

            if (order == null)
                return NotFound();

            return View(order);
        }

    }


    //public async Task<IActionResult> CheckOut()
    //{
    //    if (User.Identity?.IsAuthenticated == true)
    //    {
    //        var userId = _userManager.GetUserId(User)!;
    //        var dbCart = await _cartRepo.GetDbCartAsync(userId); //from db of cart            
    //        if (dbCart.Count == 0) return RedirectToAction("Index", "Cart");

    //        var vm = dbCart.Select(x => new CartItem
    //        {
    //            ProductId = x.ProductId,
    //            VariantId = x.VariantId,
    //            ProductName = x.Product?.Name,                 
    //            Quantity = x.Quantity,
    //            UnitPrice = x.UnitPrice,
    //            Color = x.Variant?.ColorName,
    //            Size = x.Variant?.Size
    //        }).ToList();

    //        return View(vm);
    //    }
    //    else
    //    {
    //        var cart = _cartRepo.GetSessionCart(HttpContext);
    //        if(cart.Count == 0)
    //        {
    //            return RedirectToAction("Index", "Cart");
    //        }    
    //        return View(cart);
    //    }          
    //}

    //[Authorize]
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> PlaceOrder(CheckOutVM model)
    //{
    //    if (!ModelState.IsValid) return View("Index", model);

    //    var userId = _userManager.GetUserId(User)!;

    //    int orderId;
    //    try
    //    {
    //        orderId = await _cartRepo.CreateOrderFromDbCartAsync(userId, model);
    //    }
    //    catch (InvalidOperationException)
    //    {
    //        return RedirectToAction("Index", "Cart");
    //    }
    //    catch
    //    {
    //        ModelState.AddModelError("", "Không thể tạo đơn hàng. Vui lòng thử lại.");
    //        return View("Index", model);
    //    }

    //    return RedirectToAction("ThankYou", "Orders", new { id = orderId });
    //}


    //[GET: /Orders/ThankYou/5]


    //public async Task<IActionResult> OrderIn

    //public IActionResult RemoveCart(int id)
    //{
    //    var cart = Cart;
    //    var item = cart.SingleOrDefault(p => p.ProductId == id);
    //    if(item != null)
    //    {
    //        cart.Remove(item);
    //        HttpContext.Session.SetObject(MySetting.CART_KEY, cart);
    //    }



    //    return RedirectToAction("Index", "Cart");
    //}


    //public async Task<IActionResult> AddToCart(int id, int? variantId, int quantity = 1)
    //{
    //    ProductVariant? v = null;
    //    int? resolvedVariantId = null;
    //    decimal unitPrice = 0m;

    //    if (User.Identity?.IsAuthenticated == true)
    //    {
    //        var userId = _userManager.GetUserId(User)!;

    //        if (variantId.HasValue)
    //        {
    //            v = await _context.ProductVariants
    //                  .SingleOrDefaultAsync(x => x.Id == variantId.Value && x.ProductId == id);
    //            if (v == null) return BadRequest("Biến thể không hợp lệ cho sản phẩm này.");
    //        }
    //        else
    //        {
    //            v = await _context.ProductVariants
    //                  .Where(x => x.ProductId == id)
    //                  .OrderByDescending(x => x.IsDefault)
    //                  .ThenBy(x => x.Id)
    //                  .FirstOrDefaultAsync();
    //            if (v == null) return BadRequest("Sản phẩm có nhiều biến thể, vui lòng chọn.");
    //        }

    //        resolvedVariantId = v.Id;
    //        unitPrice = v.Price;

    //        var line = await _context.Carts.FirstOrDefaultAsync(c =>
    //            c.UserId == userId && c.ProductId == id && c.VariantId == resolvedVariantId);

    //        if (line != null)
    //        {
    //            line.Quantity += quantity;
    //        }
    //        else
    //        {
    //            _context.Carts.Add(new Cart
    //            {
    //                UserId = userId,
    //                ProductId = id,
    //                VariantId = resolvedVariantId,
    //                Quantity = quantity,
    //                UnitPrice = unitPrice
    //            });
    //        }

    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }

    //    // ========== Session branch ==========
    //    var dbItem = await _context.Products
    //        .Include(p => p.Product_Imgs)
    //        .Include(p => p.Variants)
    //        .SingleOrDefaultAsync(p => p.Id == id);
    //    if (dbItem is null) return Redirect("/404");

    //    if (variantId.HasValue)
    //    {
    //        v = dbItem.Variants.FirstOrDefault(x => x.Id == variantId.Value);
    //        if (v == null) return BadRequest("Biến thể không hợp lệ cho sản phẩm này.");
    //    }
    //    else
    //    {
    //        v = dbItem.Variants
    //            .OrderByDescending(x => x.IsDefault)
    //            .ThenBy(x => x.Id)
    //            .FirstOrDefault();
    //        if (v == null) return BadRequest("Vui lòng chọn biến thể (màu/size).");
    //    }

    //    resolvedVariantId = v.Id;
    //    unitPrice = v.Price;

    //    var cart = Cart;
    //    var item = cart.SingleOrDefault(x => x.ProductId == id && x.VariantId == resolvedVariantId);
    //    if (item == null)
    //    {
    //        cart.Add(new CartItem
    //        {
    //            ProductId = dbItem.Id,
    //            VariantId = resolvedVariantId,
    //            ProductName = dbItem.Name,
    //            ProductImage = dbItem.Product_Imgs.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
    //                           ?? dbItem.Product_Imgs.FirstOrDefault()?.ImageUrl
    //                           ?? "default-image-url",
    //            UnitPrice = unitPrice,
    //            Color = v.ColorName,
    //            Size = v.Size,
    //            Quantity = quantity
    //        });
    //    }
    //    else
    //    {
    //        item.Quantity += quantity;
    //    }

    //    HttpContext.Session.SetObject(MySetting.CART_KEY, cart);
    //    return RedirectToAction(nameof(Index));
    //}


    //public async Task<IActionResult> AddToCart(int id, int variantId, int quantity = 1)
    //{

    //    if (User.Identity?.IsAuthenticated == true)
    //    {
    //        var userId = _userManager.GetUserId(User)!;

    //        // Lấy sản phẩm + variant để biết UnitPrice, Color/Size, Image nếu cần
    //        var product = await _context.Products
    //            .Include(p => p.Variants)
    //            .SingleOrDefaultAsync(p => p.Id == id);
    //        if (product is null) return Redirect("/404");


    //        // Upsert dòng giỏ: (UserId, ProductId, VariantId) là key
    //        var line = await _context.Carts
    //            .FirstOrDefaultAsync(c => c.UserId == userId
    //                                   && c.ProductId == id
    //                                   && c.VariantId == variantId
    //                                  );

    //        if (line != null)
    //        {
    //            line.Quantity += quantity;
    //            // Chính sách giá: có thể giữ snapshot cũ hoặc cập nhật theo unitPrice mới
    //            // line.UnitPrice = unitPrice;
    //        }
    //        else
    //        {
    //            _context.Carts.Add(new Cart
    //            {
    //                UserId = userId,
    //                ProductId = id,
    //                VariantId = variantId,
    //                Quantity = quantity,
    //                UnitPrice = unitPrice
    //            });
    //        }

    //        await _context.SaveChangesAsync();
    //        return RedirectToAction(nameof(Index));
    //    }

    //    // Chưa đăng nhập: session như cũ

    //    var cart = Cart; // property session
    //    var item = cart.SingleOrDefault(p => p.ProductId == id && p.VariantId == variantId);
    //    if (item is null)
    //    {
    //        var dbItem = _context.Products
    //           .Include(p => p.Product_Imgs)
    //           .Include(p => p.Variants)
    //           .SingleOrDefault(p => p.Id == id);
    //        if (dbItem is null) return Redirect("/404");

    //        //var v = (variantId.HasValue)
    //        //    ? dbItem.Variants.FirstOrDefault(x => x.Id == variantId.Value)
    //        //    : dbItem.Variants.FirstOrDefault();

    //        item = new CartItem
    //        {
    //            ProductId = dbItem.Id,
    //            ProductName = dbItem.Name,
    //            ProductImage = dbItem.Product_Imgs.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
    //                           ?? dbItem.Product_Imgs.FirstOrDefault()?.ImageUrl
    //                           ?? "default-image-url",
    //            UnitPrice = dbItem.Variants.FirstOrDefault().Price,
    //            Color = dbItem.Variants.FirstOrDefault().ColorName,
    //            Size = dbItem.Variants.FirstOrDefault().Size,

    //            VariantId = dbItem.Variants.FirstOrDefault().Id,

    //            Quantity = quantity
    //        };
    //        cart.Add(item);
    //    }
    //    else
    //    {
    //        item.Quantity += quantity;
    //    }

    //    HttpContext.Session.SetObject(MySetting.CART_KEY, cart);
    //    return RedirectToAction(nameof(Index));
    //}


    //public async Task<IActionResult> RemoveCart(int id, int? variantId = null)
    //{
    //    if (User.Identity?.IsAuthenticated == true)
    //    {
    //        var userId = _userManager.GetUserId(User)!;
    //        // Xóa trong DB
    //        var line = await _context.Carts
    //            .FirstOrDefaultAsync(c => c.UserId == userId
    //                                   && c.ProductId == id
    //                                   && (variantId == null || c.VariantId == variantId));
    //        if (line != null)
    //        {
    //            _context.Carts.Remove(line);
    //            await _context.SaveChangesAsync();
    //        }
    //    }
    //    else
    //    {
    //        // Xóa trong Session
    //        var cart = Cart;
    //        var item = cart.SingleOrDefault(p => p.ProductId == id
    //                                          && (variantId == null || p.VariantId == variantId));
    //        if (item != null)
    //        {
    //            cart.Remove(item);
    //            HttpContext.Session.SetObject(MySetting.CART_KEY, cart);
    //        }
    //    }

    //    return RedirectToAction(nameof(Index));
    //}

    //public IActionResult AddToCart(int id, int quantity = 1)
    //{
    //    var cart = Cart; 

    //    var item = cart.SingleOrDefault(p => p.ProductId == id);
    //    if (item is null)
    //    {
    //        var dbItem = _context.Products
    //           .Include(p => p.Product_Imgs)
    //           .Include(p => p.Variants)
    //           .SingleOrDefault(p => p.Id == id);
    //        if (dbItem is null)
    //        {
    //            TempData["Message"] = $"Không tìm thấy sản phẩm có mã {id}";
    //            return Redirect("/404");
    //        }

    //        item = new CartItem
    //        {
    //            ProductId = dbItem.Id,
    //            ProductImage = dbItem.Product_Imgs.FirstOrDefault()?.ImageUrl ?? "default-image-url",
    //            UnitPrice = dbItem.Variants.FirstOrDefault().Price,
    //            ProductName = dbItem.Name,
    //            Color = dbItem.Variants.FirstOrDefault().ColorName,
    //            Size = dbItem.Variants.FirstOrDefault().Size,
    //            VariantId = dbItem.Variants.FirstOrDefault().Id,
    //            Quantity = quantity
    //        };
    //        cart.Add(item);
    //    }
    //    else
    //    {
    //        item.Quantity += quantity;
    //    }

    //    HttpContext.Session.SetObject(MySetting.CART_KEY, cart);
    //    return RedirectToAction("Index", "Cart");
    //}
}
