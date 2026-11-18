# KtraBE – Website bán hàng FASTOR + quản lý đơn hàng Admin

Đây là **project mới, không liên quan đến Bakery Store**. Có **trang cửa hàng dành cho khách hàng** và **trang quản trị dành cho Admin**, dùng chung database SQL Server `MyStore_BE_Ktra`.

## Mở bằng Visual Studio 2026

1. Giải nén ZIP vào **thư mục mới** rồi mở file `KtraBE.sln`.
2. Nếu Visual Studio hỏi, chọn **Restore NuGet Packages**. Cần workload **ASP.NET and web development** và .NET Framework 4.8.
3. **Không cần chạy lại SQL** nếu bạn đã chạy `Database/01_Create_MyStore_BE_Ktra.sql` và kiểm tra thấy 31 đơn hàng; cả cửa hàng và Admin đều dùng chính database này. Nếu chuyển sang máy khác, chạy file SQL một lần trên `(localdb)\MSSQLLocalDB`.
4. Nhấn **F5**, truy cập `https://localhost:PORT/` để xem **trang chủ cửa hàng**. `PORT` là cổng do Visual Studio cấp.
5. Đăng nhập Admin qua `/Account/Login`: **admin / Admin@2026** → `/Admin/Orders`.
6. Ở cửa hàng, tạo tài khoản khách hàng mới tại `/CustomerAccount/Register`, chọn sản phẩm, thêm giỏ, checkout (thanh toán mô phỏng), xem `/CustomerOrder/OrderHistory`.

## Những gì đã làm

**Cửa hàng Customer:** Trang chủ, danh sách sản phẩm từ SQL theo danh mục, tìm kiếm và phân trang, trang chi tiết, giỏ hàng qua Session, đăng ký/đăng nhập/đăng xuất, trang tài khoản, checkout lưu đơn vào SQL Server, trang xác nhận đơn và lịch sử mua.

**Admin theo 5 slide đầu `Ktra BE`:** Danh sách đơn hàng, tìm kiếm theo tên khách/trạng thái thanh toán/ngày, phân trang **15 đơn/trang**, trang chi tiết dạng hóa đơn, cập nhật trạng thái. Giao diện danh sách không có nút Thêm/Sửa/Xóa đơn.

**Khác biệt với slide mẫu:** Bản này dùng EF6 ánh xạ bảng theo code (`Entities.cs`), **không có file `.edmx`** như ví dụ Database First trong tài liệu; và giao diện cửa hàng sử dụng mẫu FASTOR được tạo để có site đầy đủ, không phải bản gốc một website nào khác. Một số mục trong toàn bộ Checklist_BE (CRUD Admin sản phẩm/danh mục, các chức năng Customer nâng cao) **chưa thực hiện** vì nằm ngoài phạm vi đề 5 slide đã giữ.

## File code cần chỉ cho cô

- `Areas/Admin/Controllers/OrdersController.cs`: `Index`, `Details`, `UpdateStatus`.
- `Areas/Admin/Views/Orders/Index.cshtml`: trang tìm kiếm & danh sách.
- `Areas/Admin/Views/Orders/_OrderInvoice.cshtml`: chi tiết hóa đơn.
- `Areas/Admin/Views/Orders/UpdateStatus.cshtml`: form cập nhật.
- `Controllers/HomeController.cs`, `CartController.cs`, `CustomerAccountController.cs`, `CustomerOrderController.cs`: phần Customer.
- `Models/Entities.cs` & `Models/StoreModels.cs`: EF6 và ViewModels.
- `Web.config`: kết nối `MyStore_BE_Ktra` trên `(localdb)\MSSQLLocalDB`.

## Ghi chú

- SQL đã chạy trên máy trước đó có thể dùng tiếp, không tạo database mới.
- Thanh toán chỉ là mô phỏng, không có cổng thanh toán thật.
- Mã nguồn chưa được kiểm tra chạy IIS Express end-to-end trên chính máy Windows của bạn; nếu VS báo lỗi, xem Error List và Output.
