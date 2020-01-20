using System.Collections.Generic;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Form : ViewPartContainer
    {
        public string Action { get; set; }
        
        public override string OpeningTag => "form";

        public override Dictionary<string, string> InnerAttributes => new Dictionary<string, string>(base.InnerAttributes) { { "action", Action }, { "method", "post" } };
    }
}