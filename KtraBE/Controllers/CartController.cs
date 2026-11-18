using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KtraBE.Models;
namespace KtraBE.Controllers
{
    public class CartController : Controller
    {
        private readonly MyStoreContext db = new MyStoreContext();
        internal static List<ShopCartItem> GetItems(HttpSessionStateBase session)
        {
            var list = session["ShopCart"] as List<ShopCartItem>;
            if (list == null) { list = new List<ShopCartItem>(); session["ShopCart"] = list; }
            return list;
        }
        [HttpGet]
        public ActionResult Index() { return View(new ShopCartVM { Items = GetItems(Session) }); }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(int productId, int quantity = 1)
        {
            var product = db.Products.AsNoTracking().FirstOrDefault(p => p.ProductID == productId);
            if (product == null) return HttpNotFound();
            quantity = Math.Min(50, Math.Max(1, quantity));
            var items = GetItems(Session);
            var current = items.FirstOrDefault(i => i.ProductID == productId);
            if (current == null) items.Add(new ShopCartItem { ProductID = product.ProductID, ProductName = product.ProductName, ProductImage = product.ProductImage, ProductPrice = product.ProductPrice, Quantity = quantity });
            else current.Quantity = Math.Min(50, current.Quantity + quantity);
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Update(int productId, int quantity)
        {
            var items = GetItems(Session);
            var item = items.FirstOrDefault(i => i.ProductID == productId);
            if (item != null) { if (quantity < 1) items.Remove(item); else item.Quantity = Math.Min(50, quantity); }
            return RedirectToAction("Index");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Remove(int productId)
        {
            var items = GetItems(Session);
            items.RemoveAll(i => i.ProductID == productId);
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
