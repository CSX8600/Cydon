namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PageElementPermissionsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("CySys.PageElement", "ForAuthenticatedOnly", c => c.Boolean(nullable: false));
            DropColumn("World.CountryRolePageElement", "IsForAuthenticatedOnly");
        }
        
        public override void Down()
        {
            AddColumn("World.CountryRolePageElement", "IsForAuthenticatedOnly", c => c.Boolean(nullable: false));
            DropColumn("CySys.PageElement", "ForAuthenticatedOnly");
        }
    }
}
