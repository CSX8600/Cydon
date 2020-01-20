using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;
using Cydon.Data.World;

namespace Cydon.Data.CySys
{
    [Table("Page", Schema = "CySys")]
    [Unique(new string[] { "CountryID", "Name" })]
    public class Page
    {
        public long? PageID { get; set; }
        [Index("UQPage_CountryID_Name", 1, IsUnique = true)]
        public long CountryID { get; set; }
        public virtual Country Country { get; set; }
        [Index("UQPage_CountryID_Name", 2, IsUnique = true)]
        [MaxLength(25)]
        public string Name { get; set; }

        public virtual ICollection<Navigation> Navigations { get; set; }
        public virtual ICollection<PageElement> PageElements { get; set; }
    }
}
