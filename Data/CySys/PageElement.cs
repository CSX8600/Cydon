using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.World;

namespace Cydon.Data.CySys
{
    [Table("PageElement", Schema = "CySys")]
    public class PageElement
    {
        public long PageElementID { get; set; }
        public long PageID { get; set; }
        public virtual Page Page { get; set; }
        public string ElementXML { get; set; }
        public byte DisplayOrder { get; set; }
        public bool ForAuthenticatedOnly { get; set; }
        public virtual ICollection<CountryRolePageElement> CountryRolePageElements { get; set; }
    }
}
