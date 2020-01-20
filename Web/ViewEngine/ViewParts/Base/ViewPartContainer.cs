using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Xml.Linq;

namespace Web.ViewEngine.ViewParts.Base
{
    public abstract class ViewPartContainer : ViewPart
    {
        protected virtual string InnerContentTextBeforeContent { get; } = string.Empty;
        protected virtual string InnerContentTextAfterContent { get; } = string.Empty;

        public List<ViewPart> Parts = new List<ViewPart>();
        public sealed override void RenderInnerContent(HtmlTextWriter writer)
        {
            if (!string.IsNullOrEmpty(InnerContentTextBeforeContent))
            {
                writer.Write(InnerContentTextBeforeContent);
            }

            foreach(ViewPart part in Parts)
            {
                part.PageElement = PageElement;
                part.Render(writer);
            }

            if (!string.IsNullOrEmpty(InnerContentTextAfterContent))
            {
                writer.Write(InnerContentTextAfterContent);
            }
        }
    }
}