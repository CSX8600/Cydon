using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Web.ViewEngine.ViewParts.Base;

namespace Web.ViewEngine.ViewParts
{
    public class Checkbox : ViewPart
    {
        public string Text { get; set; }
        public bool Checked { get; set; }

        public Checkbox() : base()
        {
            Class.Add("form-check");
        }

        public override string OpeningTag => "div";

        protected override bool DeferIdRender => true;

        public override void RenderInnerContent(HtmlTextWriter writer)
        {
            CheckboxInternal checkboxInternal = new CheckboxInternal(Checked)
            {
                Id = Id,
                PageElement = PageElement
            };

            checkboxInternal.Render(writer);

            if (!string.IsNullOrEmpty(Text))
            {
                Label checkboxLabel = new Label()
                {
                    Class = { "form-check-label" },
                    InnerAttributes = { { "for", Id } },
                    Text = Text,
                    PageElement = PageElement
                };

                checkboxLabel.Render(writer);
            }
        }

        private class CheckboxInternal : ViewPart
        {
            private bool isChecked;
            public CheckboxInternal(bool isChecked) : base()
            {
                Class.Add("form-check-input");
                this.isChecked = isChecked;
            }

            public override string OpeningTag => "input";

            public override Dictionary<string, string> InnerAttributes
            {
                get
                {
                    Dictionary<string, string> attributes = new Dictionary<string, string>(base.InnerAttributes);
                    attributes.Add("type", "checkbox");
                    attributes.Add("name", Id);

                    if (isChecked)
                    {
                        attributes.Add("checked", string.Empty);
                    }

                    return attributes;
                }
            }
        }
    }
}