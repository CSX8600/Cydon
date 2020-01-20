using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Paragraph : ViewPart
    {
        public override string OpeningTag => "p";
        public string Text { get; set; }

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            writer.Write(Text);
        }
    }
}