using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml.Linq;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class List : ViewPart
    {
        public override string OpeningTag
        {
            get
            {
                switch(ListType)
                {
                    case ListTypes.Ordered:
                        return "ol";
                    case ListTypes.Unordered:
                        return "ul";
                }

                return "";
            }
        }
        public ListTypes ListType { get; set; }

        public List<ListItem> ListItems { get; set; } = new List<ListItem>();

        public enum ListTypes
        {
            Unordered,
            Ordered
        }

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            foreach(ListItem item in ListItems)
            {
                item.PageElement = PageElement;
                item.Render(writer);
            }
        }

        public class ListItem : ViewPartContainer
        {
            public override string OpeningTag => "li";
        }
    }
}