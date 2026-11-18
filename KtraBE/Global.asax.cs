using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using KtraBE.Models;

namespace KtraBE
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<MyStoreContext>(null); // SQL chạy riêng, EF không tự xóa/tạo DB
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
