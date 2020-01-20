using System.Collections.Generic;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Nav : ViewPartContainer
    {
        public Nav()
        {
            Class = new HashSet<string>(Class)
            {
                "navbar",
                "navbar-expand-sm",
                "navbar-dark",
                "bg-dark"
            };
        }

        public override string OpeningTag => "nav";
    }
}