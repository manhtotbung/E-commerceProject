using Microsoft.AspNetCore.Identity;
using WebBH.Areas.Data;
using WebBH.Constants;

namespace WebBH.Data
{
    public class DbSeeder
    {
        public static async Task DatabaseSeeder(IServiceProvider service)
        {
            //được dùng để lấy các dịch vụ quản lý người dùng và role từ
            //Dependency Injection container của ASP.NET Core
            var UserMgr = service.GetRequiredService<UserManager<AppUser>>(); // lấy quanr lý ng dùng
            var RoleMgr = service.GetRequiredService<RoleManager<IdentityRole>>(); // lấy về quản lý  role 

           //them vao trong database cua identity 2 role là admin và user
            await RoleMgr.CreateAsync(new IdentityRole(Roles.Admin));
            await RoleMgr.CreateAsync(new IdentityRole(Roles.User));

            //create admin
            var Admin = new AppUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FirstName = "Super",
                LastName = "Admin"
            };
            // tìm admin qua mail, nếu ko tồn tại thì thêm admin bên trên là admin
            var UserInDB = await UserMgr.FindByEmailAsync(Admin.Email);
            if (UserInDB is null)
            {
                await UserMgr.CreateAsync(Admin, "Admin@123");
                await UserMgr.AddToRoleAsync(Admin, Roles.Admin);
            }
        }
    }    
}
