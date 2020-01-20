using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;

namespace Cydon.Data.World
{
    [Table("CountryRole", Schema = "World")]
    [Unique(new string[] { "CountryID", "Name" })]
    public class CountryRole
    {
        public long? CountryRoleID { get; set; }
        [Index("UQCountryRole_CountryID_Name", 1, IsUnique = true)]
        public long CountryID { get; set; }
        public virtual Country Country { get; set; }
        [Index("UQCountryRole_CountryID_Name", 2, IsUnique = true)]
        [MaxLength(25)]
        public string Name { get; set; }
        public bool CanUpdatePermissions { get; set; }
        public bool CanAddPages { get; set; }
        public bool CanDeletePages { get; set; }

        public virtual ICollection<CountryRolePageElement> CountryRolePageElements { get; set; }
        public virtual ICollection<CountryRoleUser> CountryRoleUsers { get; set; }
    }
}
