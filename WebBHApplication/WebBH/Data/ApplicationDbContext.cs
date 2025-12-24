using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBH.Areas.Data;
using WebBH.Models;

namespace WebBH.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        //đại diện cho một bảng (table) trong cơ sở dữ liệu và cho phép bạn truy vấn,
        //thêm, sửa, xóa dữ liệu kiểu Product thông qua Entity Framework 
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants{ get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Order>Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product_Img> Product_Imgs { get; set; }



    }
}
