using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace KtraBE.Models
{
    /// <summary>
    /// Real photographic product images from Unsplash; no generated imagery.
    /// If the offline photo was downloaded, prefer the local image.
    /// </summary>
    public static class RealProductPhoto
    {
        private static readonly string[] Remote = new[]
        {
            "",
            "https://images.unsplash.com/photo-1724166595400-fdfcdb29685e?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1695774165691-8a01a6045952?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1520467795206-62e33627e6ce?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1683920999075-c5e29a0bc306?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1661246960287-c3942adab588?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1688296524835-363a81b93921?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1633354557121-98c12ac821bf?auto=format&fit=crop&w=900&q=82",
            "https://images.unsplash.com/photo-1711030238974-4e55cf76d497?auto=format&fit=crop&w=900&q=82"
        };

        public static string GetUrl(int productId, string databaseImage = null)
        {
            if (!String.IsNullOrWhiteSpace(databaseImage)) return databaseImage;
            if (productId >= 1 && productId < Remote.Length)
            {
                string virtualPath = "~/Content/products/real-" + productId + ".jpg";
                string diskPath = HostingEnvironment.MapPath(virtualPath);
                if (!String.IsNullOrWhiteSpace(diskPath) && File.Exists(diskPath))
                    return VirtualPathUtility.ToAbsolute(virtualPath);
                return Remote[productId];
            }
            return Remote[1];
        }
    }
}
