namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addseveraluniquenessconstraints : DbMigration
    {
        public override void Up()
        {
            DropIndex("World.CydonRole", new[] { "CountryID" });
            DropIndex("World.CountryRolePageElement", new[] { "CountryRoleID" });
            DropIndex("World.CountryRolePageElement", new[] { "PageElementID" });
            DropIndex("CySys.Page", new[] { "CountryID" });
            DropIndex("CySys.Navigation", new[] { "PageID" });
            DropIndex("World.CountryRoleUser", new[] { "CountryRoleID" });
            DropIndex("World.CountryRoleUser", new[] { "UserID" });
            DropIndex("Security.SitePermissionUser", new[] { "UserID" });
            DropIndex("Security.UserDiscord", new[] { "UserID" });
            AlterColumn("CySys.CacheVersion", "CacheName", c => c.String(maxLength: 100));
            AlterColumn("World.Country", "Name", c => c.String(nullable: false, maxLength: 25));
            AlterColumn("World.CydonRole", "Name", c => c.String(maxLength: 25));
            AlterColumn("CySys.Page", "Name", c => c.String(maxLength: 25));
            AlterColumn("Security.User", "Username", c => c.String(nullable: false, maxLength: 30));
            CreateIndex("CySys.CacheVersion", "CacheName", unique: true);
            CreateIndex("World.Country", "Name", unique: true);
            CreateIndex("World.CydonRole", new[] { "CountryID", "Name" }, unique: true, name: "UQCountryRole_CountryID_Name");
            CreateIndex("World.CountryRolePageElement", new[] { "CountryRoleID", "PageElementID" }, unique: true, name: "UQCountryRolePageElement_CountryRoleID_PageElementID");
            CreateIndex("CySys.Page", new[] { "CountryID", "Name" }, unique: true, name: "UQPage_CountryID_Name");
            CreateIndex("CySys.Navigation", new[] { "ParentNavigationID", "PageID" }, unique: true, name: "UQNavigation_ParentNavigationID_PageID");
            CreateIndex("World.CountryRoleUser", new[] { "CountryRoleID", "UserID" }, unique: true, name: "UQCountryRoleUser_CountryRoleID_UserID");
            CreateIndex("Security.User", "Username", unique: true);
            CreateIndex("Security.SitePermissionUser", "UserID", unique: true);
            CreateIndex("Security.UserDiscord", "UserID", unique: true);
            CreateIndex("Security.Session", "UserID", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("Security.Session", new[] { "UserID" });
            DropIndex("Security.UserDiscord", new[] { "UserID" });
            DropIndex("Security.SitePermissionUser", new[] { "UserID" });
            DropIndex("Security.User", new[] { "Username" });
            DropIndex("World.CountryRoleUser", "UQCountryRoleUser_CountryRoleID_UserID");
            DropIndex("CySys.Navigation", "UQNavigation_ParentNavigationID_PageID");
            DropIndex("CySys.Page", "UQPage_CountryID_Name");
            DropIndex("World.CountryRolePageElement", "UQCountryRolePageElement_CountryRoleID_PageElementID");
            DropIndex("World.CydonRole", "UQCountryRole_CountryID_Name");
            DropIndex("World.Country", new[] { "Name" });
            DropIndex("CySys.CacheVersion", new[] { "CacheName" });
            AlterColumn("Security.User", "Username", c => c.String(maxLength: 30));
            AlterColumn("CySys.Page", "Name", c => c.String());
            AlterColumn("World.CydonRole", "Name", c => c.String());
            AlterColumn("World.Country", "Name", c => c.String());
            AlterColumn("CySys.CacheVersion", "CacheName", c => c.String());
            CreateIndex("Security.UserDiscord", "UserID");
            CreateIndex("Security.SitePermissionUser", "UserID");
            CreateIndex("World.CountryRoleUser", "UserID");
            CreateIndex("World.CountryRoleUser", "CountryRoleID");
            CreateIndex("CySys.Navigation", "PageID");
            CreateIndex("CySys.Page", "CountryID");
            CreateIndex("World.CountryRolePageElement", "PageElementID");
            CreateIndex("World.CountryRolePageElement", "CountryRoleID");
            CreateIndex("World.CydonRole", "CountryID");
        }
    }
}
