using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;

namespace Cydon.Data.Security
{
    [Table("Session", Schema = "Security")]
    [Unique(new string[] { "UserID" })]
    public class Session
    {
        public long SessionID { get; set; }
        [Required]
        public string SessionStateID { get; set; }
        [Index(IsUnique = true)]
        public long UserID { get; set; }
        public DateTime Expiration { get; set; }
    }
}
