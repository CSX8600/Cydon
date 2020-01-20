using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Button : ViewPart
    {
        public override string OpeningTag => "button";

        public string Type { get; set; }
        public string OnClick { get; set; }
        public string Text { get; set; }
        public List<ViewPart> ViewPartsBeforeText { get; set; } = new List<ViewPart>();
        public List<ViewPart> ViewPartsAfterText { get; set; } = new List<ViewPart>();

        public override Dictionary<string, string> InnerAttributes => new Dictionary<string, string>(base.InnerAttributes) { { "type", Type }, { "onclick", OnClick } };

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            foreach(ViewPart part in ViewPartsBeforeText)
            {
                part.PageElement = PageElement;
                part.Render(writer);
            }

            writer.Write(Text);

            foreach(ViewPart part in ViewPartsAfterText)
            {
                part.PageElement = PageElement;
                part.Render(writer);
            }
        }
    }
}