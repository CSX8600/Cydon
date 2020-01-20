using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;

namespace Cydon.Data.CySys
{
    [Table("Navigation", Schema = "CySys")]
    [Unique(new string[] { "ParentNavigationID", "PageID" })]
    public class Navigation
    {
        public long? NavigationID { get; set; }
        [Index("UQNavigation_ParentNavigationID_PageID", 1, IsUnique = true)]
        public long? ParentNavigationID { get; set; }
        public virtual Navigation ParentNavigation { get; set; }
        [Index("UQNavigation_ParentNavigationID_PageID", 2, IsUnique = true)]
        public long PageID { get; set; }
        public virtual Page Page { get; set; }
        public string Text { get; set; }

        public virtual ICollection<Navigation> ChildNavigations { get; set; }
    }
}
