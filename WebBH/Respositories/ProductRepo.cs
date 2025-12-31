using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using WebBH.Data;
using WebBH.Models;

namespace WebBH.Respositories
{
    public class ProductRepo : IProductRepo //Kế thừa từ IHomeRepo để thực hiện các phương thức truy vấn dữ liệu
    {
        private readonly ApplicationDbContext _context; // Khởi tạo ApplicationDbContext để truy cập cơ sở dữ liệu this is called 
        public ProductRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetFilteredProductAsync(FilteredProduct f)
        {
            var query = _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Variants)
                .Include(p => p.Product_Imgs)
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(f.SearchTerm))
            {
                var kw = f.SearchTerm.Trim();
                query = query.Where(p =>
                    (p.Name != null && p.Name.Contains(kw)) ||
                    (p.Description != null && p.Description.Contains(kw)));
            }

            // Category
            if (f.CategoryId?.Any() == true)
                query = query.Where(p => f.CategoryId.Contains(p.CategoryId));

            // ---- Size + Color
            bool hasSize = f.Size?.Any() == true;
            bool hasColor = f.Color?.Any() == true;

            var sizes = hasSize ? f.Size.Select(s => s.Trim()).ToList() : null;
            var colors = hasColor ? f.Color.Select(c => c.Trim().ToLower()).ToList() : null; // form có thể gửi "Black" → "black"

            if (hasSize || hasColor)
            {
                query = query.Where(p => p.Variants.Any(v =>
                       (!hasSize || (v.Size != null && sizes!.Contains(v.Size)))
                    && (!hasColor || (v.ColorId != null && colors!.Contains(v.ColorId.ToLower())))
                ));
            }
        
            if (f.PriceRange?.Any() == true)
            {
                var r = f.PriceRange[0]; // radio → 1 giá trị
                query = query.Where(p => p.Variants.Any()); // tránh MIN trên tập rỗng

                const decimal L300 = 300_000M; //M -> iteral là decimal.
                const decimal L500 = 500_000M;

                if (r == "Under300")
                    query = query.Where(p => p.Variants.Min(v => v.Price) < L300);
                else if (r == "300to500")
                    query = query.Where(p =>
                        p.Variants.Min(v => v.Price) >= L300 &&
                        p.Variants.Min(v => v.Price) <= L500);
                else if (r == "Above500")
                    query = query.Where(p => p.Variants.Min(v => v.Price) > L500); 
            }

            return await query.ToListAsync();
        }

        //public async Task<List<Product>> GetFilteredProductAsync(FilteredProduct filteredProduct)
        //{
        //    var query = _context.Products
        //       .Include(p => p.Categories) 
        //       .Include(p => p.Variants) 
        //       .Include(p => p.Product_Imgs) 
        //       .AsQueryable();

        //        if (!string.IsNullOrEmpty(filteredProduct.SearchTerm))
        //        {
        //            query = query.Where(p => (p.Name !=null && p.Name.Contains(filteredProduct.SearchTerm)) ||
        //                                     (p.Description!=null && p.Description.Contains(filteredProduct.SearchTerm)));
        //        }
        //        if (filteredProduct.CategoryId != null && filteredProduct.CategoryId.Any())
        //        {
        //            query = query.Where(p => filteredProduct.CategoryId.Contains(p.CategoryId));
        //        }
        //        if(filteredProduct.Size!= null && filteredProduct.Size.Any())
        //        {
        //            query = query.Where(p => p.Variants.Any(v => filteredProduct.Size.Contains(v.Size)));               
        //        }
        //        if (filteredProduct.Color != null && filteredProduct.Color.Any())
        //        {
        //            query = query.Where(p => p.Variants.Any(v => filteredProduct.Color.Contains(v.ColorId)));
        //        }
        //    //price
        //    //if (filteredProduct.PriceRange != null && filteredProduct.PriceRange.Contains("Under300"))
        //    //{
        //    //    query = query.Where(p => p.Variants.Any(v => v.Price < 300000));
        //    //}
        //    //if(filteredProduct.PriceRange != null && filteredProduct.PriceRange.Contains("300to500"))
        //    //{
        //    //    query = query.Where(p => p.Variants.Any(v => v.Price >= 300000 && v.Price <= 500000));
        //    //}
        //    //if (filteredProduct.PriceRange != null && filteredProduct.PriceRange.Contains("Above500"))
        //    //{
        //    //    query = query.Where(p => p.Variants.Any(v => v.Price >= 500000));
        //    //}
        //    // PRICE — lọc theo MIN(Variant.Price) của từng sản phẩm
        //    if (filteredProduct.PriceRange?.Any() == true)
        //    {
        //        var r = filteredProduct.PriceRange[0];                 // radio => 1 giá trị
        //        query = query.Where(p => p.Variants.Any()); // tránh MIN trên tập rỗng

        //        if (r == "Under300")
        //            query = query.Where(p => p.Variants.Min(v => v.Price) < 300_000M);

        //        else if (r == "300to500")
        //            query = query.Where(p =>
        //                p.Variants.Min(v => v.Price) >= 300_000M &&
        //                p.Variants.Min(v => v.Price) <= 500_000M);

        //        else if (r == "Above500")
        //            query = query.Where(p => p.Variants.Min(v => v.Price) > 500_000M); // dùng '>' để 500k thuộc nhóm giữa
        //    }
        //    return await query.ToListAsync(); // Trả về danh sách sản phẩm đã lọc
        //}

        public async Task<ProductDetailViewModel> GetProductByIdAsync(int id)
        {
            var Product = await _context.Products
                .Include(p => p.Categories) 
                .Include(p => p.Variants) 
                .Include(p => p.Product_Imgs) 
                .SingleOrDefaultAsync(p => p.Id == id);

            var RelatedProduct = _context.Products
                .Include(p => p.Product_Imgs)
                .Where(p => p.CategoryId == Product.CategoryId && p.Id != id)
                .Take(4)
                .ToList();
            
            var ViewModel = new ProductDetailViewModel
            {
                Product =  Product,
                RelatedProduct = RelatedProduct
            };

            return ViewModel;
        }
    }
}