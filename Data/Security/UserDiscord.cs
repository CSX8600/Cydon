using System;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;

namespace Cydon.Data.Security
{
    [Table("UserDiscord", Schema = "Security")]
    [Unique(new string[] { "UserID" })]
    public class UserDiscord
    {
        public long? UserDiscordID { get; set; }
        [Index(IsUnique = true)]
        public long? UserID { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expiration { get; set; }

        public virtual User User { get; set; }
    }
}
