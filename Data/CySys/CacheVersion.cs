using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Cydon.Data.Base;

namespace Cydon.Data.CySys
{
    [Table("CacheVersion", Schema = "CySys")]
    [Unique(new string[] { "CacheName" })]
    public class CacheVersion
    {
        public long? CacheVersionID { get; set; }
        [Index(IsUnique = true)]
        [MaxLength(100)]
        public string CacheName { get; set; }
        public DateTime NextRefreshTime { get; set; }
    }
}
