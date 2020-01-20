using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;
using Cydon.Data.CySys;

namespace Cydon.Data.World
{
    [Table("Country", Schema = "World")]
    [Unique(new string[] { "Name" })]
    
    public class Country
    {
        public long? CountryID { get; set; }
        [Required]
        [MaxLength(25)]
        [Index(IsUnique = true)]
        public string Name { get; set; }

        public virtual ICollection<Page> Pages { get; set; }
        public virtual ICollection<CountryRole> CountryRoles { get; set; }
    }
}
