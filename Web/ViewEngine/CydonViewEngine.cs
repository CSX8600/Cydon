using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Web.ViewEngine
{
    public class CydonViewEngine : IViewEngine
    {
        private Dictionary<string, Type> viewTypesByName = new Dictionary<string, Type>();
        private static Dictionary<string, Type> partialViewsByName = new Dictionary<string, Type>();

        public CydonViewEngine()
        {
            Type partialViewSystemType = typeof(CydonPartialViewSystem);
            foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type type in assembly.GetTypes().Where(t => t != partialViewSystemType && partialViewSystemType.IsAssignableFrom(t)))
                {
                    partialViewsByName.Add(type.Name, type);
                }
            }
        }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            if (partialViewsByName.ContainsKey(partialViewName))
            {
                Type partialViewType = partialViewsByName[partialViewName];
                IView partialView = (IView)Activator.CreateInstance(partialViewType);
                return new ViewEngineResult(partialView, this);
            }

            return new ViewEngineResult(new List<string>() { "Partial View Registry did not contain the specified partial view" });
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return new ViewEngineResult(new List<string>() { "This View Engine does not currently support Views " });
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            
        }
    }
}