using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KtraBE.Models;
namespace KtraBE.Controllers
{
    public class CustomerAccountController : Controller
    {
        private readonly MyStoreContext db = new MyStoreContext();
        [HttpGet, AllowAnonymous] public ActionResult Register() { return View(new CustomerRegisterVM()); }
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Register(CustomerRegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);
            model.Username = model.Username.Trim();
            if (db.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại."); return View(model);
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                db.Users.Add(new UserAccount { Username = model.Username, Password = PasswordHasher.Hash(model.Password), UserRole = "C" });
                db.Customers.Add(new Customer { Username = model.Username, CustomerName = model.CustomerName.Trim(), CustomerPhone = model.CustomerPhone.Trim(), CustomerEmail = model.CustomerEmail, CustomerAddress = model.CustomerAddress.Trim() });
                db.SaveChanges(); transaction.Commit();
            }
            FormsAuthentication.SetAuthCookie(model.Username, false);
            TempData["ShopSuccess"] = "Tạo tài khoản thành công!";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet, AllowAnonymous] public ActionResult Login() { return View(new LoginVM()); }
        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Username == model.Username && u.UserRole == "C");
            if (user == null || !PasswordHasher.Verify(model.Password, user.Password))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu chưa đúng."); return View(model);
            }
            FormsAuthentication.SetAuthCookie(user.Username, false);
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult Profile()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login");
            var customer = db.Customers.AsNoTracking().FirstOrDefault(c => c.Username == User.Identity.Name);
            if (customer == null) return new HttpStatusCodeResult(403);
            return View(customer);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut(); Session.Clear();
            return RedirectToAction("Index", "Home");
        }
        protected override void Dispose(bool disposing) { if (disposing) db.Dispose(); base.Dispose(disposing); }
    }
}
