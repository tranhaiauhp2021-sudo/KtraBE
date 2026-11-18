using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KtraBE.Models;
namespace KtraBE.Controllers
{
    public class CustomerOrderController : Controller
    {
        private readonly MyStoreContext db = new MyStoreContext();
        private Customer CurrentCustomer()
        {
            return User.Identity.IsAuthenticated ? db.Customers.FirstOrDefault(c => c.Username == User.Identity.Name) : null;
        }
        [HttpGet]
        public ActionResult Checkout()
        {
            var customer = CurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "CustomerAccount");
            var cart = CartController.GetItems(Session);
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            return View(new CheckoutVM { AddressDelivery = customer.CustomerAddress, Cart = new ShopCartVM { Items = cart } });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutVM vm)
        {
            var customer = CurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "CustomerAccount");
            var cart = CartController.GetItems(Session);
            if (!cart.Any()) return RedirectToAction("Index", "Cart");
            if (!new[] {"Giao tiêu chuẩn","Giao nhanh"}.Contains(vm.DeliveryMethod)) ModelState.AddModelError("DeliveryMethod","Hình thức giao không hợp lệ.");
            if (!new[] {"Tiền mặt","Chuyển khoản"}.Contains(vm.PaymentMethod)) ModelState.AddModelError("PaymentMethod","Hình thức thanh toán không hợp lệ.");
            if (!ModelState.IsValid) { vm.Cart = new ShopCartVM { Items = cart }; return View(vm); }
            var ids = cart.Select(c => c.ProductID).ToArray();
            var products = db.Products.AsNoTracking().Where(p => ids.Contains(p.ProductID)).ToList();
            if (products.Count != ids.Length)
            {
                ModelState.AddModelError("", "Một sản phẩm không còn tồn tại, vui lòng kiểm tra lại giỏ hàng.");
                vm.Cart = new ShopCartVM { Items = cart }; return View(vm);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                var order = new Order
                {
                    CustomerID = customer.CustomerID, OrderDate = DateTime.Today,
                    PaymentStatus = "Chưa thanh toán", OrderStatus = "Chờ xử lý",
                    AddressDelivery = vm.AddressDelivery.Trim(), PaymentMethod = vm.PaymentMethod, DeliveryMethod = vm.DeliveryMethod
                };
                foreach (var item in cart)
                {
                    var product = products.First(p => p.ProductID == item.ProductID);
                    int quantity = Math.Max(1, Math.Min(50, item.Quantity));
                    order.OrderDetails.Add(new OrderDetail { ProductID = item.ProductID, Quantity = quantity, UnitPrice = product.ProductPrice });
                    order.TotalAmount += quantity * product.ProductPrice;
                }
                db.Orders.Add(order);
                db.SaveChanges(); transaction.Commit();
                Session.Remove("ShopCart");
                return RedirectToAction("OrderSuccess", new { id = order.OrderID });
            }
        }
        [HttpGet]
        public ActionResult OrderSuccess(int id)
        {
            var customer = CurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "CustomerAccount");
            var order = db.Orders.Include(o => o.OrderDetails.Select(d => d.Product)).AsNoTracking().FirstOrDefault(o => o.OrderID == id && o.CustomerID == customer.CustomerID);
            if (order == null) return HttpNotFound();
            return View(order);
        }
        [HttpGet]
        public ActionResult OrderHistory()
        {
            var customer = CurrentCustomer();
            if (customer == null) return RedirectToAction("Login", "CustomerAccount");
            var orders = db.Orders.AsNoTracking().Where(o => o.CustomerID == customer.CustomerID).OrderByDescending(o => o.OrderID).Take(50).ToList();
            return View(orders);
        }
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
