using System.Collections.Generic;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class TextBox : ViewPart
    {
        public string Text { get; set; }
        public override string OpeningTag => "input";
        public override Dictionary<string, string> InnerAttributes// => new Dictionary<string, string>(base.InnerAttributes) { { "type", "text" }, { "name", Id }, { "value", Text } };
        {
            get
            {
                Dictionary<string, string> attr = base.InnerAttributes;
                attr["type"] = "text";
                attr["name"] = Id;
                attr["value"] = Text;
                return attr;
            }
        }
    }
}