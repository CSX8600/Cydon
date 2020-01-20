//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Cydon.Data.Form
//{
//    [Table("FormElementInstance", Schema = "Form")]
//    public class FormElementInstance
//    {
//        public long? FormElementInstanceID { get; set; }
//        public long FormInstanceID { get; set; }
//        public FormInstance FormInstance { get; set; }
//        public long? FormElementID { get; set; }
//        public FormElement FormElement { get; set; }
//        public string ValueXML { get; set; }
//    }
//}
