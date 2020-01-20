using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class ComboBox : ViewPart
    {
        public override string OpeningTag => "select";
        public override Dictionary<string, string> InnerAttributes => new Dictionary<string, string>(base.InnerAttributes) { { "name", Id } };
        public List<Item> SelectItems { get; set; } = new List<Item>();

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            foreach(Item item in SelectItems)
            {
                writer.AddAttribute("value", item.Value);
                if (item.Selected)
                {
                    writer.AddAttribute("selected", "");
                }
                
                writer.RenderBeginTag("option");
                writer.Write(item.Text);
                writer.RenderEndTag();
            }
        }

        public class Item
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public bool Selected { get; set; }
        }
    }
}