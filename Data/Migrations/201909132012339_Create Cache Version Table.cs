namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCacheVersionTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "CySys.CacheVersion",
                c => new
                    {
                        CacheVersionID = c.Long(nullable: false, identity: true),
                        CacheName = c.String(),
                        NextRefreshTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.CacheVersionID);
            
        }
        
        public override void Down()
        {
            DropTable("CySys.CacheVersion");
        }
    }
}
