using System.Data.Entity;
using Cydon.Data.CySys;
using Cydon.Data.Security;
using Cydon.Data.World;

namespace Cydon.Data.Base
{
    public class Context : DbContext
    {
        public Context() : base(Config.INSTANCE.ConnectionString) { }

        // CySys
        public virtual DbSet<CacheVersion> CacheVersions { get; set; }
        public virtual DbSet<Navigation> Navigations { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageElement> PageElements { get; set; }
        // Security
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<SitePermissionUser> SitePermissionUsers { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<UserDiscord> UserDiscords { get; set; }
        // World
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<CountryRole> CountryRoles { get; set; }
        public virtual DbSet<CountryRolePageElement> CountryRolePageElements { get; set; }
        public virtual DbSet<CountryRoleUser> CountryRoleUsers { get; set; }
        // Form
        //public virtual DbSet<Form.Form> Forms { get; set; }
        //public virtual DbSet<FormElement> FormElements { get; set; }
        //public virtual DbSet<FormInstance> FormInstances { get; set; }
        //public virtual DbSet<FormElementInstance> FormElementInstances { get; set; }
    }
}
