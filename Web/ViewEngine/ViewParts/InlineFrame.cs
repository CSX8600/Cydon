using System.Collections.Generic;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class InlineFrame : ViewPart
    {
        public InlineFrame()
        {
            Class.Add("embed-responsive");
            Class.Add("embed-responsive-1by1");
        }

        private InlineFrameInternal inlineFrameInternal = new InlineFrameInternal();
        public string Source
        {
            get { return inlineFrameInternal.Source; }
            set { inlineFrameInternal.Source = value; }
        }

        public override string OpeningTag => "div";

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            base.RenderInnerContent(writer);
            inlineFrameInternal.Render(writer);
        }

        private class InlineFrameInternal : ViewPart
        {
            public InlineFrameInternal()
            {
                Class.Add("embed-responsive-item");
            }

            public override string OpeningTag => "iframe";

            public string Source { get; set; }

            public override Dictionary<string, string> InnerAttributes => new Dictionary<string, string>(base.InnerAttributes)
            {
                { "src", Source }
            };
        }
    }
}