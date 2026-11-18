using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using KtraBE.Models;

namespace KtraBE.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                using (var db = new MyStoreContext())
                {
                    if (db.Users.AsNoTracking().Any(x => x.Username == User.Identity.Name && x.UserRole == "A"))
                        return RedirectToAction("Index", "Orders", new { area = "Admin" });
                }
            }
            return View(new LoginVM());
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Login(LoginVM model)
        {
            if (!ModelState.IsValid) return View(model);
            using (var db = new MyStoreContext())
            {
                var user = db.Users.AsNoTracking().FirstOrDefault(x => x.Username == model.Username && x.UserRole == "A");
                if (user == null || !PasswordHasher.Verify(model.Password, user.Password))
                {
                    ModelState.AddModelError("", "Sai tài khoản/mật khẩu hoặc không có quyền Admin.");
                    return View(model);
                }
            }
            FormsAuthentication.SetAuthCookie(model.Username, false);
            return RedirectToAction("Index", "Orders", new { area = "Admin" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
