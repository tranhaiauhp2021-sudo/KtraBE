# ĐỐI CHIẾU CHECKLIST_BE – phạm vi 5 slide Admin

Nguồn: `TaiLieu/LTW_Checklist_BE_Goc.xlsx`, sheet `Checklist_BE`.

| Dòng Excel | Mục cần kiểm | Tình trạng trong source | Minh chứng |
|---|---|---|---|
| 3 | Sử dụng DB thực hành | Có schema MyStore bản kiểm tra, mở rộng OrderStatus/PaymentMethod/DeliveryMethod | `Database/01_Create_MyStore_BE_Ktra.sql` |
| 5 | Khóa ngoại FK giữa các bảng | Có User→Customer, Category→Product, Customer→Order, Order→OrderDetail, Product→OrderDetail | SQL `FOREIGN KEY` + `Models/Entities.cs` |
| 6 | Admin dùng Layout chung | Có `_AdminLayout.cshtml` qua `Views/_ViewStart.cshtml` trong Area | `Areas/Admin/Views/Shared/_AdminLayout.cshtml` |
| 26 | Index: danh sách đơn hàng KHÔNG có nút Thêm/Sửa/Xóa | Đã viết View danh sách chỉ có liên kết XEM CHI TIẾT, tìm kiếm và phân trang | `Areas/Admin/Views/Orders/Index.cshtml` |
| 27 | Detail: thông tin khách hàng và danh sách chi tiết theo kiểu hóa đơn | Có Partial View hóa đơn; hiển thị mã đơn, khách, sản phẩm, số lượng, đơn giá, thành tiền | `_OrderInvoice.cshtml`, `Details.cshtml` |
| Đề slide 5 | Tìm tên KH / trạng thái thanh toán / ngày đặt | Có biểu mẫu GET, LINQ filter và EF6 | `OrdersController.Index`, `Index.cshtml` |
| Đề slide 5 | Phân trang 15 đơn/trang | `PageSize = 15`, `Skip`/`Take` | `OrdersController.Index` |
| Đề slide 5 | Không có kết quả thì hiện KHÔNG CÓ | Có dòng thông báo | `Index.cshtml` |
| Đề slide 5 | Cập nhật trạng thái tại trang chi tiết | Có nút sang `UpdateStatus`, GET + POST + antiforgery | `OrdersController.UpdateStatus`, `UpdateStatus.cshtml` |

**Quan trọng:** Mục Customer của Checklist (dòng 28–65) và các phần Admin danh mục, sản phẩm, kho... nằm **ngoài phạm vi bài kiểm tra 5 slide** đã chọn; không đánh dấu hoàn thành các phần đó. Danh sách này là kiểm tra **mã nguồn**, KHÔNG phải xác nhận đã chạy thành công trong VS/SQL trên Windows.

## Phần cửa hàng được bổ sung (ngoài phạm vi 5 slide Admin)

| Hạng mục Customer | Trong project | File kiểm tra |
|---|---|---|
| Trang chủ/danh mục/tìm kiếm/phân trang | Có | `Controllers/HomeController.cs`, `Views/Home/Index.cshtml` |
| Trang chi tiết sản phẩm | Có | `HomeController.Details`, `Views/Home/Details.cshtml` |
| Giỏ hàng | Có, Session | `Controllers/CartController.cs`, `Views/Cart/Index.cshtml` |
| Đăng ký, đăng nhập, đăng xuất | Có | `Controllers/CustomerAccountController.cs` |
| Đặt hàng và xác nhận | Có, SQL; thanh toán mô phỏng | `Controllers/CustomerOrderController.cs` |
| Xem lịch sử mua | Có, danh sách cơ bản | `Views/CustomerOrder/OrderHistory.cshtml` |
| Tìm kiếm nâng cao lịch sử theo mã/sản phẩm, lọc tab, mua lại | Chưa có | Ngoài phạm vi 5 slide Admin đã giữ |
| Admin CRUD sản phẩm/danh mục/tài khoản | Chưa có | Ngoài phạm vi đề 5 slide Admin |

Đây là đối chiếu **mã nguồn**, không phải kết quả kiểm thử chạy thực tế trên Visual Studio của người dùng.
