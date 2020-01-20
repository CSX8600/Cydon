using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Cydon.Data.Base;
using Web.Base;
using Web.ViewEngine;

namespace Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static Thread cacheThread;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Add(new CydonViewEngine());
            cacheThread = Cache.Initialize();
            cacheThread.Start();
            var config = Config.INSTANCE;
        }

        protected void Application_End()
        {
            cacheThread.Abort();
        }
    }
}
