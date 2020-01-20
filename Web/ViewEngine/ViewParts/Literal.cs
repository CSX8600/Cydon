using System;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Literal : ViewPart
    {
        public override bool CustomRender => true;
        public override string OpeningTag => throw new NotImplementedException();

        public string LiteralString { get; set; }

        protected override void PerformCustomRender(HtmlTextWriter writer)
        {
            writer.Write(LiteralString);
        }
    }
}