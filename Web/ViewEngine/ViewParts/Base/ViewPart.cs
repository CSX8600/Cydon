using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Cydon.Data.CySys;

namespace Web.ViewEngine.ViewParts.Base
{
    public abstract class ViewPart
    {
        public PageElement PageElement { get; set; }
        public abstract string OpeningTag { get; }
        public virtual bool CustomRender => false;

        protected virtual void PerformCustomRender(HtmlTextWriter writer) { throw new NotImplementedException("Custom Render function not implemented"); }
        

        public virtual Dictionary<string, string> InnerAttributes { get; } = new Dictionary<string, string>();

        public virtual Dictionary<string, string> Data { get; } = new Dictionary<string, string>();

        protected virtual void PreRender() { }

        public string Id { get; set; }

        protected virtual bool DeferIdRender { get; } = false;

        public HashSet<string> Class { get; set; } = new HashSet<string>();

        public virtual void RenderInnerContent(HtmlTextWriter writer) { }

        public void Render(HtmlTextWriter writer)
        {
            PreRender();

            if (CustomRender)
            {
                PerformCustomRender(writer);
                return;
            }

            if (!string.IsNullOrEmpty(Id) && !DeferIdRender)
            {
                writer.AddAttribute("id", Id);
            }

            StringBuilder classNameBuilder = new StringBuilder();
            for(int i = 0; i < Class.Count; i++)
            {
                if (i != 0)
                {
                    classNameBuilder.Append(" ");
                }

                classNameBuilder.Append(Class.ElementAt(i));
            }

            writer.AddAttribute("class", classNameBuilder.ToString());

            foreach(KeyValuePair<string, string> kvp in InnerAttributes)
            {
                writer.AddAttribute(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<string, string> kvp in Data)
            {
                writer.AddAttribute(string.Format("data-{0}", kvp.Key), kvp.Value);
            }

            writer.RenderBeginTag(OpeningTag);

            RenderInnerContent(writer);

            writer.RenderEndTag();
        }
    }
}