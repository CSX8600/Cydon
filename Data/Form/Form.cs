//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using Cydon.Data.CySys;

//namespace Cydon.Data.Form
//{
//    [Table("Form", Schema = "Form")]
//    public class Form
//    {
//        public long? FormID { get; set; }
//        public long PageElementID { get; set; }
//        public virtual PageElement PageElement { get; set; }
//        public string FormName { get; set; }
//        public bool Publish { get; set; }

//        public virtual ICollection<FormElement> FormElements { get; set; }
//        public virtual ICollection<FormInstance> FormInstances { get; set; }
//    }
//}
