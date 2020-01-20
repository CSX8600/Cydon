using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Web.ViewEngine.ViewParts;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.PageContentViewParts
{
    public class EmbeddedGoogleForm : PageContentViewPart
    {
        private string _url = "";
        public override void ReadFromXML(XElement xElement)
        {
            _url = xElement.Attribute("url")?.Value;
        }

        public override void UpdateFromFormInput(Dictionary<string, object> values)
        {
            _url = values["url"] as string;

            if (_url.Contains("viewform"))
            {
                int viewFormIndex = _url.IndexOf("viewform");
                _url = _url.Substring(0, viewFormIndex + 8);
                _url += "?embedded=true";
            }
        }

        public override void WriteToXML(XElement xElement)
        {
            xElement.Add(new XAttribute("url", _url));
        }

        protected override IEnumerable<ViewPart> GetEditorViewParts()
        {
            Div formGroup = new Div()
            {
                Class = { "form-group" }
            };

            Label forLabel = new Label()
            {
                Text = "Shareable Google Form Link",
                InnerAttributes = { { "for", "url" } }
            };

            TextBox textBox = new TextBox()
            {
                Text = _url,
                Id = "url",
                Class = { "form-control" }
            };

            if (ReadOnly)
            {
                textBox.InnerAttributes.Add("readonly", "");
            }

            Label helpLabel = new Label()
            {
                Class = { "text-danger" },
                Data = { { "validate-control-for", "url" } }
            };

            formGroup.Parts.Add(forLabel);
            formGroup.Parts.Add(textBox);
            formGroup.Parts.Add(helpLabel);

            yield return formGroup;
        }

        protected override IEnumerable<ViewPart> GetNormalViewParts()
        {
            yield return new InlineFrame()
            {
                Source = _url
            };
        }
    }
}