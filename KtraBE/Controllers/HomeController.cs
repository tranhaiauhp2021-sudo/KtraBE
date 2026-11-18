using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using KtraBE.Models;
namespace KtraBE.Controllers
{
    // Giao diện mua sắm cho khách hàng, hiển thị dữ liệu thật từ SQL Server.
    public class HomeController : Controller
    {
        private readonly MyStoreContext db = new MyStoreContext();
        [HttpGet]
        public ActionResult Index(string keyword, int? categoryId, int page = 1)
        {
            const int pageSize = 8;
            var query = db.Products.Include(p => p.Category).AsNoTracking().AsQueryable();
            if (!String.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();
                query = query.Where(p => p.ProductName.Contains(keyword) || p.ProductDecription.Contains(keyword));
            }
            if (categoryId.HasValue) query = query.Where(p => p.CategoryID == categoryId.Value);
            int total = query.Count();
            int count = Math.Max(1, (int)Math.Ceiling((double)total / pageSize));
            page = Math.Max(1, Math.Min(page, count));
            return View(new ShopHomeVM
            {
                Keyword = keyword, CategoryId = categoryId, Page = page, PageCount = count, TotalCount = total,
                Categories = db.Categories.AsNoTracking().OrderBy(c => c.CategoryName).ToList(),
                Products = query.OrderBy(p => p.ProductID).Skip((page - 1)*pageSize).Take(pageSize).ToList()
            });
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            var product = db.Products.Include(p => p.Category).AsNoTracking().FirstOrDefault(p => p.ProductID == id);
            if (product == null) return HttpNotFound();
            return View(new ShopProductVM
            {
                Product = product,
                Related = db.Products.AsNoTracking().Where(p => p.CategoryID == product.CategoryID && p.ProductID != id).Take(4).ToList()
            });
        }
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
