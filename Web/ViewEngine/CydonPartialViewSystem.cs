using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine
{
    public abstract class CydonPartialViewSystem : IView
    {
        public List<ViewPart> Parts = new List<ViewPart>();
        public void Render(ViewContext viewContext, TextWriter writer)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter(writer);

            FillPartsList(viewContext);

            foreach(ViewPart part in Parts)
            {
                part.Render(htmlTextWriter);
            }
        }

        protected abstract void FillPartsList(ViewContext context);
    }
}