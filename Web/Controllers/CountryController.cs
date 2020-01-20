using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml.Linq;
using Cydon.Data.Base;
using Cydon.Data.CySys;
using Cydon.Data.World;
using Web.Base;
using Web.ViewEngine.ViewParts.Base;

namespace Web.Controllers
{
    public class CountryController : BaseController
    {        
        // GET: Country
        public ActionResult Index(string country, string page, long? id)
        {
            if (string.IsNullOrEmpty(country))
            {
                country = "Cydon";
            }

            if (string.IsNullOrEmpty(page))
            {
                page = "Index";
            }

            Context context = new Context();
            Country selectedCountry = context.Countries.FirstOrDefault(c => c.Name == country);
            if (selectedCountry == null)
            {
                return HttpNotFound("Country not found");
            }

            Cydon.Data.CySys.Page selectedPage = selectedCountry.Pages.FirstOrDefault(p => p.Name == page);
            if (selectedPage == null)
            {
                return HttpNotFound("Page not found");
            }

            MemoryStream stream = new MemoryStream();
            HtmlTextWriter textWriter = new HtmlTextWriter(new StreamWriter(stream));
            foreach(PageElement pageElement in selectedPage.PageElements.OrderBy(pe => pe.DisplayOrder))
            {
                if (pageElement.ForAuthenticatedOnly && UserID == null)
                {
                    continue;
                }

                XDocument elementXML = XDocument.Parse(pageElement.ElementXML);
                Type elementType = Type.GetType(elementXML.Root.Attribute("type").Value);

                if (!typeof(PageContentViewPart).IsAssignableFrom(elementType))
                {
                    continue;
                }

                PageContentViewPart viewPart = (PageContentViewPart)Activator.CreateInstance(elementType);
                viewPart.PageElement = pageElement;
                viewPart.ReadFromXML(elementXML.Root);
                viewPart.Render(textWriter);
            }

            StreamReader reader = new StreamReader(stream);
            textWriter.Flush();
            stream.Position = 0;
            ViewData["Content"] = reader.ReadToEnd();
            reader.Close();
            textWriter.Close();

            if (Request.QueryString.AllKeys.Contains("signoutReason"))
            {
                ViewData["signoutSuccessful"] = true;
            }

            return View();
        }
    }
}