using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;
using Cydon.Data.World;

namespace Cydon.Data.Security
{
    [Table("User", Schema = "Security")]
    [Unique(new string[] { "Username" })]
    public class User
    {
        public long? UserID { get; set; }
        [MaxLength(30, ErrorMessage = "Username must be equal to or less than 30 characters")]
        [Required]
        [Index(IsUnique = true)]
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public bool IsDiscordUser { get; set; }

        public virtual ICollection<SitePermissionUser> SitePermissionUsers { get; set; }
        public virtual ICollection<UserDiscord> UserDiscords { get; set; }
        public virtual ICollection<CountryRoleUser> CountryRoleUsers { get; set; }
    }
}
