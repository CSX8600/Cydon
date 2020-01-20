using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Web.ViewEngine.ViewParts;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.PageContentViewParts
{
    public class Jumbotron : PageContentViewPart
    {
        public string Text { get; set; } = "";
        public override void ReadFromXML(XElement element)
        {
            Text = element.Value;
        }

        public override void UpdateFromFormInput(Dictionary<string, object> values)
        {
            if (values.ContainsKey("Text"))
            {
                Text = values["Text"] as string;
            }
        }

        public override void WriteToXML(XElement element)
        {
            element.Value = Text;
        }

        protected override IEnumerable<ViewPart> GetEditorViewParts()
        {
            Div div = new Div()
            {
                Class = { "form-group" }
            };

            Header header = new Header()
            {
                HeaderType = Header.HeaderTypes.H4,
                Text = "Jumbotron"
            };

            div.Parts.Add(header);

            Label label = new Label()
            {
                Text = "Text",
                InnerAttributes = { { "for", "Text" } },
            };

            TextBox text = new TextBox()
            {
                Id = "Text",
                Text = Text,
                Class = { "form-control" }
            };

            if (ReadOnly)
            {
                text.InnerAttributes.Add("readonly", "");
            }

            Label validationError = new Label()
            {
                Class = { { "text-danger" } },
                Data = { { "validate-message-for", "Text" } }
            };

            div.Parts.AddRange(new List<ViewPart>() { label, text, validationError });

            yield return div;
        }

        protected override IEnumerable<ViewPart> GetNormalViewParts()
        {
            Div jumbotronDiv = new Div()
            {
                Class = { "jumbotron" }
            };

            jumbotronDiv.Parts.Add(new Paragraph() { Text = Text });

            yield return jumbotronDiv;
        }
    }
}