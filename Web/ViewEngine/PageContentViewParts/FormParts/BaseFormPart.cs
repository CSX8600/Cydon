//using System;
//using System.Collections.Generic;
//using System.Xml.Linq;
//using Cydon.Data.Form;
//using Web.ViewEngine.ViewParts.Base;
//using static Cydon.Data.Form.FormElement;

//namespace Web.ViewEngine.PageContentViewParts.FormParts
//{
//    public abstract class BaseFormPart
//    {
//        public static Dictionary<ElementTypes, Type> ElementTypesTypeLookup = new Dictionary<ElementTypes, Type>()
//        {
//            { ElementTypes.RadioButton, typeof(RadioButtonFormPart) }
//        };

//        public FormElement FormElement { protected get; set; }
//        public abstract void ReadFromXML(XElement xElement);
//        public abstract void WriteToXML(XElement xElement);
//        public abstract IEnumerable<ViewPart> GetNormalParts();
//        public abstract IEnumerable<ViewPart> GetEditableParts();
//        public abstract void UpdateValuesFromForm(Dictionary<string, object> values);
//        protected string GenerateID(string id)
//        {
//            return $"formelement-{FormElement.FormID}-{FormElement.FormElementID}-{id}";
//        }
//    }
//}