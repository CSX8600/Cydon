//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Xml.Linq;
//using Cydon.Data.Base;
//using Cydon.Data.Form;
//using Web.Base;
//using Web.ViewEngine.ViewParts;
//using Web.ViewEngine.ViewParts.Base;

//namespace Web.ViewEngine.PageContentViewParts.FormParts
//{
//    public class Form : PageContentViewPart
//    {
//        private Context _context = null;
//        private Cydon.Data.Form.Form _form = null;
//        private Dictionary<long, BaseFormPart> _formElementParts = new Dictionary<long, BaseFormPart>();
//        private string FormName;
//        private bool Published;
//        private Cydon.Data.Form.Form GetFormObject()
//        {
//            if (PageElement == null)
//            {
//                return null;
//            }

//            if (_form != null)
//            {
//                return _form;
//            }

//            if (_context == null)
//            {
//                _context = new Context();
//            }

//            _form = _context.Forms.FirstOrDefault(f => f.PageElementID == PageElement.PageElementID);
//            if (_form == null)
//            {
//                _form = new Cydon.Data.Form.Form()
//                {
//                    PageElementID = PageElement.PageElementID
//                };

//                _context.Forms.Add(_form);

//                _context.SaveChanges();
//            }

//            foreach(Cydon.Data.Form.FormElement formElement in _form.FormElements)
//            {
//                Type formElementType = BaseFormPart.ElementTypesTypeLookup[formElement.ElementType];
//                BaseFormPart formPart = (BaseFormPart)Activator.CreateInstance(formElementType);
//                formPart.FormElement = formElement;
//                XElement xElement = XElement.Parse(formElement.ElementXML);
//                formPart.ReadFromXML(xElement);
//                _formElementParts.Add(formElement.FormElementID.Value, formPart);
//            }

//            return _form;
//        }

//        public override void UpdateFromFormInput(Dictionary<string, object> values)
//        {
//            Cydon.Data.Form.Form formToUse = GetFormObject();
//            if (values.ContainsKey("name"))
//            {
//                FormName = values["name"] as string;
//            }

//            if (values.ContainsKey("published") && bool.TryParse(values["published"].ToString(), out bool published))
//            {
//                Published = published;
//            }

//            foreach(IGrouping<string, KeyValuePair<string, object>> formElementPart in values.Where(val => val.Key.StartsWith("formelement-")).GroupBy(kvp =>
//            {
//                string[] parts = kvp.Key.Split('-');
//                return $"{parts[0]}-{parts[1]}-{parts[2]}";
//            }))
//            {
//                string[] parts = formElementPart.Key.Split('-');
//                if (!long.TryParse(parts[2], out long formElementID))
//                {
//                    continue;
//                }

//                if (!_formElementParts.ContainsKey(formElementID))
//                {
//                    continue;
//                }

//                BaseFormPart baseFormPart = _formElementParts[formElementID];
//                baseFormPart.UpdateValuesFromForm(formElementPart.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
//            }
//        }

//        protected override IEnumerable<ViewPart> GetEditorViewParts()
//        {
//            Cydon.Data.Form.Form formToUse = GetFormObject();
//            yield return BuildNamePartEditor(formToUse?.FormName);
//            yield return BuildPublishPartEditor(formToUse?.Publish ?? false);
//            yield return BuildFormElementsEditor();
//            yield return BuildAddFormElementModal();
//        }

//        private Div BuildNamePartEditor(string formName)
//        {
//            string fieldName = GenerateID("name");
//            Div nameDiv = new Div()
//            {
//                Class = { "form-group" }
//            };

//            Label nameLabel = new Label()
//            {
//                InnerAttributes = { { "for", fieldName } },
//                Text = "Name"
//            };

//            TextBox nameTextBox = new TextBox()
//            {
//                Id = fieldName,
//                Class = { "form-control" },
//                Text = formName

//            };

//            Label nameError = new Label()
//            {
//                Class = { "text-danger" },
//                Data = { { "validate-message-for", fieldName } }
//            };

//            nameDiv.Parts.AddRange(new List<ViewPart>() { nameLabel, nameTextBox, nameError });
//            return nameDiv;
//        }

//        private Div BuildPublishPartEditor(bool published)
//        {
//            string fieldName = GenerateID("published");
//            Div div = new Div()
//            {
//                Class = { "form-control" }
//            };

//            Checkbox checkbox = new Checkbox()
//            {
//                Id = fieldName,
//                Text = "Published",
//                Checked = published
//            };

//            Label publishedError = new Label()
//            {
//                Class = { "text-danger" },
//                Data = { { "validate-message-for", "published" } }
//            };

//            div.Parts.Add(checkbox);
//            div.Parts.Add(publishedError);

//            return div;
//        }

//        private Div BuildFormElementsEditor()
//        {
//            Div outterDiv = new Div();

//            Div elements = new Div()
//            {
//                Class = { "border", "border-dark", "p-2" }
//            };

//            Label header = new Label()
//            {
//                Text = "Form Elements"
//            };

//            outterDiv.Parts.Add(header);

//            foreach(KeyValuePair<long, BaseFormPart> baseFormPartByFormElementID in _formElementParts)
//            {
//                Div formElementDiv = new Div()
//                {
//                    Class = { "border", "border-dark", "p-2" }
//                };

//                Button deleteElement = new Button()
//                {
//                    Type = "button",
//                    Class = { "btn", "btn-danger" },
//                    OnClick = "formeditor.deleteFormElement(" + baseFormPartByFormElementID.Key + ", $(this));"
//                };

//                formElementDiv.Parts.AddRange(baseFormPartByFormElementID.Value.GetEditableParts());
//                elements.Parts.Add(formElementDiv);
//            }

//            Button addButton = new Button()
//            {
//                Class = { "btn", "btn-primary", "mt-2" },
//                Type = "button",
//                OnClick = "$('#" + GenerateID("addmodal") + "').modal('show');",
//                ViewPartsBeforeText = new List<ViewPart>()
//                {
//                    new Span() { Class = { "fas", "fa-plus-circle" } }
//                },
//                Text = " Add"
//            };

//            elements.Parts.Add(addButton);

//            outterDiv.Parts.Add(elements);

//            return outterDiv;
//        }

//        private Div BuildAddFormElementModal()
//        {
//            Div modalDiv = new Div()
//            {
//                Id = GenerateID("addmodal"),
//                Class = { "modal", "fade" },
//                InnerAttributes = { { "role", "dialog" } }
//            };

//            Div modalDialogDiv = new Div()
//            {
//                Class = { "modal-dialog" },
//                InnerAttributes = { { "role", "document" } }
//            };
//            modalDiv.Parts.Add(modalDialogDiv);

//            Div modalContentDiv = new Div()
//            {
//                Class = { "modal-content" }
//            };
//            modalDialogDiv.Parts.Add(modalContentDiv);

//            Div modalHeaderDiv = new Div()
//            {
//                Class = { "modal-header" }
//            };

//            Header headerText = new Header()
//            {
//                Class = { "modal-title" },
//                HeaderType = Header.HeaderTypes.H4,
//                Text = "<strong>Add</strong> Form Element"
//            };
//            modalHeaderDiv.Parts.Add(headerText);
//            modalContentDiv.Parts.Add(modalHeaderDiv);

//            Div modalBody = new Div()
//            {
//                Class = { "modal-body" }
//            };

//            ViewParts.Form formSelection = new ViewParts.Form()
//            {
//                Id = GenerateID("addmodal-form"),
//                Action = "."
//            };

//            string selectionID = GenerateID("addmodal-selection");
//            Div selectionDiv = new Div()
//            {
//                Class = { "form-group" }
//            };
//            Label selectionLabel = new Label()
//            {
//                InnerAttributes = { { "for", selectionID } },
//                Text = "Form Element"
//            };
//            ComboBox selectionComboBox = new ComboBox()
//            {
//                Id = selectionID,
//                Class = { "form-control" }
//            };
//            foreach(Tuple<string, int> formPart in GetFormParts())
//            {
//                selectionComboBox.SelectItems.Add(new ComboBox.Item()
//                {
//                    Text = formPart.Item1,
//                    Value = formPart.Item2.ToString()
//                });
//            }
//            Label selectionError = new Label()
//            {
//                Class = { "text-danger" },
//                Data = { { "validate-message-for", selectionID } }
//            };
//            selectionDiv.Parts.AddRange(new List<ViewPart>()
//            {
//                selectionLabel,
//                selectionComboBox,
//                selectionError
//            });
//            formSelection.Parts.Add(selectionDiv);

//            formSelection.Parts.Add(new Hidden()
//            {
//                Id = GenerateID("addmodal-formid"),
//                Value = GetFormObject().FormID.Value.ToString()
//            });

//            modalBody.Parts.Add(formSelection);
//            modalContentDiv.Parts.Add(modalBody);

//            Div modalFooter = new Div()
//            {
//                Class = { "modal-footer" }
//            };

//            Button cancelButton = new Button()
//            {
//                Type = "button",
//                Class = { "btn", "btn-outline-primary" },
//                OnClick = "$('#" + GenerateID("addmodal") + "').modal('hide');",
//                ViewPartsBeforeText = new List<ViewPart>()
//                {
//                    new Span()
//                    {
//                        Class = { "fas", "fa-ban" }
//                    }
//                },
//                Text = " Cancel"
//            };

//            Button saveButton = new Button()
//            {
//                Type = "button",
//                Class = { "btn", "btn-primary" },
//                OnClick = "modalSave('/PageEditor/FormElementAdd/' + $('#CountryID').val(), $(this));",
//                ViewPartsBeforeText = new List<ViewPart>()
//                {
//                    new Span()
//                    {
//                        Class = { "fas", "fa-save" }
//                    }
//                },
//                Text = " Save"
//            };

//            modalFooter.Parts.Add(cancelButton);
//            modalFooter.Parts.Add(saveButton);
//            modalContentDiv.Parts.Add(modalFooter);

//            return modalDiv;
//        }

//        private IEnumerable<Tuple<string, int>> GetFormParts()
//        {
//            foreach(FormElement.ElementTypes elementTypes in Enum.GetValues(typeof(FormElement.ElementTypes)))
//            {
//                yield return new Tuple<string, int>(elementTypes.ToString().ToDisplayString(), (int)elementTypes);
//            }
//        }

//        protected override IEnumerable<ViewPart> GetNormalViewParts()
//        {
//            throw new NotImplementedException();
//        }

//        // We use data, so we don't need to store xml.  However, these are
//        // functionally save and load methods, so we'll use them as that
//        public override void ReadFromXML(XElement xElement)
//        {
            
//        }

//        public override void WriteToXML(XElement xElement)
//        {
//            Cydon.Data.Form.Form formToUse = GetFormObject();

//            if (formToUse != null)
//            {
//                formToUse.FormName = FormName;
//                formToUse.Publish = Published;

//                foreach(FormElement formElement in formToUse.FormElements)
//                {
//                    if (_formElementParts.ContainsKey(formElement.FormElementID.Value))
//                    {
//                        BaseFormPart baseFormPart = _formElementParts[formElement.FormElementID.Value];
//                        XElement newXml = new XElement("element");
//                        baseFormPart.WriteToXML(newXml);
//                        formElement.ElementXML = newXml.ToString();
//                    }
//                }

//                _context.SaveChanges();
//            }
//        }

//        private string GenerateID(string controlID)
//        {
//            return $"form-{GetFormObject().FormID}-{controlID}";
//        }
//    }
//}