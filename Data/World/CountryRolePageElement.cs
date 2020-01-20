using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;
using Cydon.Data.CySys;

namespace Cydon.Data.World
{
    [Table("CountryRolePageElement", Schema = "World")]
    [Unique(new string[] { "CountryRoleID", "PageElementID" })]
    public class CountryRolePageElement
    {
        public long? CountryRolePageElementID { get; set; }
        [Index("UQCountryRolePageElement_CountryRoleID_PageElementID", 1, IsUnique = true)]
        public long CountryRoleID { get; set; }
        public virtual CountryRole CountryRole { get; set; }
        [Index("UQCountryRolePageElement_CountryRoleID_PageElementID", 2, IsUnique = true)]
        public long PageElementID { get; set; }
        public virtual PageElement PageElement { get; set; }
        public bool CanView { get; set; }
        public bool CanEdit { get; set; }
    }
}
