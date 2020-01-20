using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Country",
                url: "Country/{country}/{page}/{action}/{id}",
                defaults: new { controller = "Country", country = UrlParameter.Optional, page = UrlParameter.Optional, action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "PageEditor",
                url: "PageEditor/{action}/{countryid}/{id}",
                defaults: new { controller = "PageEditor", action = "Index", countryid = UrlParameter.Optional, id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Country", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
