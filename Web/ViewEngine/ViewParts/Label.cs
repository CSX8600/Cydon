using System.Web.UI;
using System.Xml.Linq;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Label : ViewPart
    {
        public string Text { get; set; }
        public override string OpeningTag => "label";

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            writer.Write(Text);
        }
    }
}