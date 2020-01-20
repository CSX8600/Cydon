using System.Collections.Generic;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Hidden : ViewPart
    {
        public string Value { get; set; }
        public override string OpeningTag => "input";

        public override Dictionary<string, string> InnerAttributes => new Dictionary<string, string>(base.InnerAttributes) { { "type", "hidden" }, { "name", Id }, { "value", Value } };
    }
}