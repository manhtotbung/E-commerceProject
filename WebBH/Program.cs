using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBH;
using WebBH.Areas.Data;
using WebBH.Data;
using WebBH.Respositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IProductRepo, ProductRepo>();//tạo avatar của product để inject vào HomeController( truyền vào trong contructer) tương tự như homerepo = new HomeRepo(context);
builder.Services.AddScoped<ICartRepo, CartRepo>();
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//cho nay de su dung duoc userManager va roleManager
builder.Services.AddIdentity<AppUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI(); 

builder.Services.AddControllersWithViews();

//session builder 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
//session builder end 

var app = builder.Build();

//Một số service như UserManager, RoleManager được đăng ký theo lifetime là Scoped
//→  phải tạo scope để lấy được chúng
//using (var Scope = app.Services.CreateScope())
//{
//    await DbSeeder.DatabaseSeeder(Scope.ServiceProvider); // databaseseeder là hàm mình viết
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
//xác định ng dùng hiện tại
app.UseAuthentication();
//kiểm tra xem ai đang đăng nhập
app.UseAuthorization();
//session
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
