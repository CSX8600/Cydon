//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Cydon.Data.Form
//{
//    [Table("FormElement", Schema = "Form")]
//    public class FormElement
//    {
//        public long? FormElementID { get; set; }
//        public long FormID { get; set; }
//        public virtual Form Form { get; set; }
//        public enum ElementTypes
//        {
//            RadioButton,
//            TextField
//        }
//        public ElementTypes ElementType { get; set; }
//        public string ElementXML { get; set; }
//    }
//}
