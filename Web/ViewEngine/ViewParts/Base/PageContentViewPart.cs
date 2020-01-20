using System.Collections.Generic;
using System.Web.UI;
using System.Xml.Linq;

namespace Web.ViewEngine.ViewParts.Base
{
    public abstract class PageContentViewPart : ViewPart
    {
        public bool EditorMode { get; set; }
        public bool ReadOnly { get; set; }
        public PageContentViewPart() : base() { }
        public override string OpeningTag => "div";

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            IEnumerable<ViewPart> parts = EditorMode ? GetEditorViewParts() : GetNormalViewParts();

            foreach(ViewPart part in parts)
            {
                part.PageElement = PageElement;
                part.Render(writer);
            }
        }

        public abstract void ReadFromXML(XElement xElement);
        public abstract void WriteToXML(XElement xElement);
        public abstract void UpdateFromFormInput(Dictionary<string, object> values);

        protected abstract IEnumerable<ViewPart> GetNormalViewParts();
        protected abstract IEnumerable<ViewPart> GetEditorViewParts();
    }
}