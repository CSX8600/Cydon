using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    /// <summary>
    /// The form-group class is not necessary
    /// </summary>
    public class RadioButton : ViewPart
    {
        public RadioButton() : base()
        {
            Class.Add("form-check");
        }

        public override string OpeningTag => "div";
        public bool Inline { get; set; }
        public string Text { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }

        protected override void PreRender()
        {
            if (Inline)
            {
                Class.Add("form-check-inline");
            }
        }

        protected override bool DeferIdRender => true;

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            RadioButtonInternal radioButtonInternal = new RadioButtonInternal()
            {
                Id = Id,
                Name = Name,
                Value = Value,
                Checked = Checked
            };

            radioButtonInternal.Render(writer);

            Label label = new Label()
            {
                Class = { "form-check-label" },
                InnerAttributes = { { "for", Id } },
                Text = Text
            };
            label.Render(writer);
        }

        private class RadioButtonInternal : ViewPart
        {
            public RadioButtonInternal() : base()
            {
                Class.Add("form-check-input");
            }

            public override string OpeningTag => "input";
            public string Name { get; set; }
            public string Value { get; set; }
            public bool Checked { get; set; }

            public override Dictionary<string, string> InnerAttributes
            {
                get
                {
                    Dictionary<string, string> attributes = new Dictionary<string, string>(base.InnerAttributes)
                    {
                        { "type", "radio" },
                        { "name", Name },
                        { "value", Value }
                    };

                    if (Checked)
                    {
                        attributes.Add("checked", string.Empty);
                    }

                    return attributes;
                }
            }
        }
    }
}