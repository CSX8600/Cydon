//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Cydon.Data.Security;

//namespace Cydon.Data.Form
//{
//    [Table("FormInstance", Schema = "Form")]
//    public class FormInstance
//    {
//        public long? FormInstanceID { get; set; }
//        public long FormID { get; set; }
//        public virtual Form Form { get; set; }
//        public long UserID { get; set; }
//        public virtual User User { get; set; }
//    }
//}
