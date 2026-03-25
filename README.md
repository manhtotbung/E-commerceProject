# E-commerceProject

Dự án xây dựng website thương mại điện tử dựa trên .NET (C#, ASP.NET Core MVC) đóng gói với Docker. Giao diện kết hợp HTML/CSS/JavaScript.

## 1. Key feature (Chức năng nổi bật)

- Quản lý sản phẩm: CRUD, tìm kiếm, danh mục.
- Quản lý user/khách hàng: Đăng ký, đăng nhập, phân quyền.
- Giỏ hàng & thanh toán: Thêm/xoá sản phẩm, đặt đơn, xác nhận.
- Quản lý đơn hàng: Lịch sử, trạng thái, cập nhật đơn.
- Trang khách hàng: Hiển thị sản phẩm, tìm kiếm, lọc, trang chủ...
- Trang Admin: Quản lý sản phẩm, user, đơn hàng, báo cáo.
- Phụ trợ: Quản lý profile, đổi mật khẩu, phân trang, thông báo.

## 2. Hướng dẫn cài đặt & triển khai

### Yêu cầu
- Visual Studio 2022+ (hoặc .NET 6 SDK+)
- Docker (dùng container)
- SQL Server
- Git

### Cài đặt local không dùng Docker

1. Clone repo:
   ```sh
   git clone https://github.com/manhtotbung/E-commerceProject.git
   cd E-commerceProject/WebBH
   ```
2. Mở solution `WebBHApplication.sln` với Visual Studio, restore NuGet.
3. Cấu hình file `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=TenDatabase;User Id=sa;Password=matkhaucuaban;"
     }
   }
   ```
4. Khởi tạo database (dùng migration hoặc thủ công).
5. Chạy ứng dụng (F5 hoặc `dotnet run` trong `/WebBH`).

### Cài đặt và deploy bằng Docker

1. Build image:
   ```sh
   docker build -t e-commerce:latest .
   ```
2. Run container:
   ```sh
   docker run -e "ConnectionStrings__DefaultConnection=..." -p 8080:80 e-commerce:latest
   ```
3. Truy cập: http://localhost:8080

### Biến cấu hình quan trọng
- **ConnectionStrings:DefaultConnection**: Kết nối SQL Server.
- **Logging**: Log lỗi.
- **Authentication/Authorization**: Tùy mức bảo mật.

## 3. Đóng góp & phát triển

- Fork, tạo nhánh, gửi pull request hoặc issue.
- Đọc code trong từng thư mục để hiểu cấu trúc/luồng dữ liệu.
