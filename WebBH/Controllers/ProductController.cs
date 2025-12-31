using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebBH.Models;
using WebBH.Respositories;

namespace WebBH.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepo _productRepo; //_productRepo = null  private readonly IproductRepo _productRepo = new productRepo();

        //Vì ASP.NET MVC/Core không cho bạn tự new một productRepo() [productRepo này là đối tượng trong program.cs] trong controller nữa
        //(vì như vậy là gắn chặt, khó bảo trì), nên hệ thống sẽ tự tạo sẵn một đối tượng productRepo rồi đưa vào ProductController.

        public ProductController(IProductRepo productRepo) // hệ thống sẽ truyền productRepo vào đây
        {
            _productRepo = productRepo; //gán giá trị
        }

        public async Task<IActionResult> Index(string searchTerm, List<int> categoryId, List<string> size, List<string> priceRange, List<string> color)
        {            
            var filteredProduct = new FilteredProduct
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                Size = size,
                PriceRange = priceRange,
                Color = color
            };

            var products = await _productRepo.GetFilteredProductAsync(filteredProduct);

            // Store current search term in ViewBag to maintain state
            ViewBag.CurrentSearchTerm = searchTerm;
            ViewBag.CurrentCategoryIds = categoryId;
            ViewBag.CurrentSizes = size;
            ViewBag.CurrentPriceRange = priceRange;
            ViewBag.CurrentColor = color;

            // If it's an AJAX request, return only the partial view
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ProductList", products);
            }

            return View(products);
        }
        public async Task<IActionResult> ProductDetail(int id)
        {
           var data = await _productRepo.GetProductByIdAsync(id);
            if (data == null)
            {
                var errorMessage = "Sản phẩm không tồn tại.";
                // Nếu là AJAX request, trả về JSON với thông báo lỗi
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = errorMessage });
                }
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return PartialView("_Error", errorMessage);
                }
                // Nếu không phải AJAX request, trả về trang lỗi
                else
                {
                    return Redirect("/404.cshtml");
                }
            }
            return View(data);
        }
    }
}