using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;

namespace Cydon.Data.Security
{
    [Table("SitePermissionUser", Schema = "Security")]
    [Unique(new string[] { "UserID" })]
    public class SitePermissionUser
    {
        public long? SitePermissionUserID { get; set; }
        [Index(IsUnique = true)]
        public long UserID { get; set; }
        public virtual User User { get; set; }
        public bool CanAddCountries { get; set; }
        public bool CanDeleteCountries { get; set; }
        public bool CanManagePermissions { get; set; }
    }
}
