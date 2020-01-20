using System.Collections.Generic;
using System.Xml.Linq;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Anchor : ViewPartContainer
    {
        public override string OpeningTag => "a";
        public string Href { get; set; }
        public string TextBeforeContent { get; set; }
        public string TextAfterContent { get; set; }

        protected override string InnerContentTextBeforeContent => TextBeforeContent;
        protected override string InnerContentTextAfterContent => TextAfterContent;

        public override Dictionary<string, string> InnerAttributes => new Dictionary<string, string>()
        {
            { "href", Href }
        };
    }
}