using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cydon.Data.Base;
using Cydon.Data.Security;

namespace Cydon.Data.World
{
    [Table("CountryRoleUser", Schema = "World")]
    [Unique(new string[] { "CountryRoleID", "UserID" })]
    public class CountryRoleUser
    {
        public long? CountryRoleUserID { get; set; }
        [Index("UQCountryRoleUser_CountryRoleID_UserID", 1, IsUnique = true)]
        public long CountryRoleID { get; set; }
        public virtual CountryRole CountryRole { get; set; }
        [Index("UQCountryRoleUser_CountryRoleID_UserID", 2, IsUnique = true)]
        public long UserID { get; set; }
        public virtual User User { get; set; }
    }
}
