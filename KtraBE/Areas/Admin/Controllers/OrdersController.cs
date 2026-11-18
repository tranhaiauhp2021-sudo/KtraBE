using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using KtraBE.Models;

namespace KtraBE.Areas.Admin.Controllers
{
    [AdminOnly]
    public class OrdersController : Controller
    {
        private readonly MyStoreContext db = new MyStoreContext();
        private const int PageSize = 15; // Theo file Ktra BE: 15 đơn hàng / trang.

        // GET: /Admin/Orders
        // Danh sách: tìm theo tên KH, trạng thái thanh toán, ngày đặt hàng; phân trang.
        [HttpGet]
        public ActionResult Index(string customerName, string paymentStatus, DateTime? orderDate, int page = 1)
        {
            var query = db.Orders.Include(o => o.Customer).AsQueryable();

            if (!String.IsNullOrWhiteSpace(customerName))
            {
                customerName = customerName.Trim();
                query = query.Where(o => o.Customer.CustomerName.Contains(customerName));
            }
            if (!String.IsNullOrWhiteSpace(paymentStatus))
                query = query.Where(o => o.PaymentStatus == paymentStatus);

            if (orderDate.HasValue)
            {
                DateTime day = orderDate.Value.Date;
                query = query.Where(o => DbFunctions.TruncateTime(o.OrderDate) == day);
            }

            int total = query.Count();
            int pageCount = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Max(1, Math.Min(page, pageCount));

            var model = new OrderSearchVM
            {
                CustomerName = customerName,
                PaymentStatus = paymentStatus,
                OrderDate = orderDate,
                Page = page,
                PageSize = PageSize,
                TotalCount = total,
                PageCount = pageCount,
                Orders = query.OrderByDescending(o => o.OrderDate)
                    .ThenByDescending(o => o.OrderID)
                    .Skip((page - 1) * PageSize).Take(PageSize).ToList()
            };
            return View(model);
        }

        // GET: /Admin/Orders/Details/5
        // Chi tiết: thông tin KH + dòng hàng giống hóa đơn, dữ liệu có Include.
        [HttpGet]
        public ActionResult Details(int id)
        {
            var order = db.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound("Không tìm thấy đơn hàng.");
            return View(order);
        }

        // GET: /Admin/Orders/UpdateStatus/5
        [HttpGet]
        public ActionResult UpdateStatus(int id)
        {
            var order = db.Orders.Include(o => o.Customer).FirstOrDefault(o => o.OrderID == id);
            if (order == null) return HttpNotFound("Không tìm thấy đơn hàng.");
            var vm = new UpdateOrderStatusVM
            {
                OrderID = order.OrderID,
                CustomerName = order.Customer.CustomerName,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus
            };
            SetStatusOptions(vm);
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(UpdateOrderStatusVM model)
        {
            var allowedOrder = new[] { "Chờ xử lý", "Đang xử lý", "Đang giao", "Đã giao", "Đã hủy" };
            var allowedPayment = new[] { "Chưa thanh toán", "Đã thanh toán", "Đã hoàn tiền" };
            if (!allowedOrder.Contains(model.OrderStatus))
                ModelState.AddModelError("OrderStatus", "Trạng thái đơn hàng không hợp lệ.");
            if (!allowedPayment.Contains(model.PaymentStatus))
                ModelState.AddModelError("PaymentStatus", "Trạng thái thanh toán không hợp lệ.");

            var order = db.Orders.Include(o => o.Customer).FirstOrDefault(o => o.OrderID == model.OrderID);
            if (order == null) return HttpNotFound("Không tìm thấy đơn hàng.");

            if (!ModelState.IsValid)
            {
                model.CustomerName = order.Customer.CustomerName;
                SetStatusOptions(model);
                return View(model);
            }

            // Chỉ cập nhật 2 thuộc tính được phép, không bind OrderID/CustomerID/TotalAmount từ form.
            order.OrderStatus = model.OrderStatus;
            order.PaymentStatus = model.PaymentStatus;
            db.SaveChanges();
            TempData["Success"] = "Đã cập nhật trạng thái đơn hàng #" + order.OrderID;
            return RedirectToAction("Details", new { id = order.OrderID });
        }

        private void SetStatusOptions(UpdateOrderStatusVM model)
        {
            ViewBag.OrderStatusOptions = new SelectList(new[]
            {
                "Chờ xử lý", "Đang xử lý", "Đang giao", "Đã giao", "Đã hủy"
            }, model.OrderStatus);
            ViewBag.PaymentStatusOptions = new SelectList(new[]
            {
                "Chưa thanh toán", "Đã thanh toán", "Đã hoàn tiền"
            }, model.PaymentStatus);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
