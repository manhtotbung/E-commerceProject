using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBH.Extensions;
using WebBH.Models;
using WebBH.Respositories;

public class CartViewComponent : ViewComponent
{
    private readonly ICartRepo _cartRepo;

    public CartViewComponent(ICartRepo cartRepo)
    {
        _cartRepo = cartRepo;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        int qty = 0;
        decimal total = 0m;

        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            // Lấy các dòng giỏ từ DB và tính tổng
            var lines = await _cartRepo.GetDbCartAsync(userId);
            qty = lines.Sum(x => x.Quantity);
            total = lines.Sum(x => x.Quantity * x.UnitPrice);
        }
        else
        {
            var cart = HttpContext.Session.GetObject<List<CartItem>>(MySetting.CART_KEY) ?? new();
            qty = cart.Sum(p => p.Quantity);
            total = cart.Sum(p => p.TotalPrice);
        }

        return View("CartPanel", new CartModel { Quantity = qty, Total = total });
    }
}


