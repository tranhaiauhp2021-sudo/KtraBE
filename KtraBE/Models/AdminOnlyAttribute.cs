using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KtraBE.Models
{
    // Không chỉ kiểm tra cookie: xác minh quyền A trong database ở mỗi request.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AdminOnlyAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null || httpContext.User == null ||
                httpContext.User.Identity == null || !httpContext.User.Identity.IsAuthenticated)
                return false;

            string username = httpContext.User.Identity.Name;
            using (var db = new MyStoreContext())
                return db.Users.AsNoTracking().Any(x => x.Username == username && x.UserRole == "A");
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult(403, "Chỉ Admin được truy cập");
                return;
            }
            filterContext.Result = new RedirectResult("~/Account/Login");
        }
    }
}
