using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebBH.Models;
using WebBH.Respositories;

namespace WebBH.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;   
        private readonly IProductRepo _productRepo;

        public HomeController(ILogger<HomeController> logger, IProductRepo productRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepo.GetFilteredProductAsync(new FilteredProduct());
            return View(products);
        }

        [Route("/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
