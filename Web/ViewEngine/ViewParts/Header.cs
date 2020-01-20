using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Header : ViewPart
    {
        public override string OpeningTag => HeaderType.ToString().ToLower();

        public string Text { get; set; }

        public HeaderTypes HeaderType { get; set; }

        public enum HeaderTypes
        {
            H1,
            H2,
            H3,
            H4,
            H5,
            H6
        }

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            writer.Write(Text);
        }
    }
}