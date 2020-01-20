namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Moresetupobjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "World.Country",
                c => new
                    {
                        CountryID = c.Long(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CountryID);
            
            CreateTable(
                "World.CydonRole",
                c => new
                    {
                        CountryRoleID = c.Long(nullable: false, identity: true),
                        CountryID = c.Long(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CountryRoleID)
                .ForeignKey("World.Country", t => t.CountryID, cascadeDelete: true)
                .Index(t => t.CountryID);
            
            CreateTable(
                "World.CountryRolePageElement",
                c => new
                    {
                        CountryRolePageElementID = c.Long(nullable: false, identity: true),
                        CountryRoleID = c.Long(nullable: false),
                        PageElementID = c.Long(nullable: false),
                        IsForAuthenticatedOnly = c.Boolean(nullable: false),
                        CanView = c.Boolean(nullable: false),
                        CanEdit = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CountryRolePageElementID)
                .ForeignKey("World.CydonRole", t => t.CountryRoleID, cascadeDelete: true)
                .ForeignKey("CySys.PageElement", t => t.PageElementID, cascadeDelete: true)
                .Index(t => t.CountryRoleID)
                .Index(t => t.PageElementID);
            
            CreateTable(
                "CySys.PageElement",
                c => new
                    {
                        PageElementID = c.Long(nullable: false, identity: true),
                        PageID = c.Long(nullable: false),
                        ElementXML = c.String(),
                    })
                .PrimaryKey(t => t.PageElementID)
                .ForeignKey("CySys.Page", t => t.PageID, cascadeDelete: true)
                .Index(t => t.PageID);
            
            CreateTable(
                "CySys.Page",
                c => new
                    {
                        PageID = c.Long(nullable: false, identity: true),
                        CountryID = c.Long(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.PageID)
                .ForeignKey("World.Country", t => t.CountryID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "CySys.Navigation",
                c => new
                    {
                        NavigationID = c.Long(nullable: false, identity: true),
                        ParentNavigationID = c.Long(),
                        PageID = c.Long(nullable: false),
                        Text = c.String(),
                        ParentNavigation_NavigationID = c.Long(),
                    })
                .PrimaryKey(t => t.NavigationID)
                .ForeignKey("CySys.Navigation", t => t.ParentNavigation_NavigationID)
                .ForeignKey("CySys.Page", t => t.PageID, cascadeDelete: true)
                .Index(t => t.PageID)
                .Index(t => t.ParentNavigation_NavigationID);
            
            CreateTable(
                "World.CountryRoleUser",
                c => new
                    {
                        CountryRoleUserID = c.Long(nullable: false, identity: true),
                        CountryRoleID = c.Long(nullable: false),
                        UserID = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.CountryRoleUserID)
                .ForeignKey("World.CydonRole", t => t.CountryRoleID, cascadeDelete: true)
                .ForeignKey("Security.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.CountryRoleID)
                .Index(t => t.UserID);
            
            CreateTable(
                "Security.SitePermissionUser",
                c => new
                    {
                        SitePermissionUserID = c.Long(nullable: false, identity: true),
                        UserID = c.Long(nullable: false),
                        CanAddCountries = c.Boolean(nullable: false),
                        CanDeleteCountries = c.Boolean(nullable: false),
                        CanManagePermissions = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SitePermissionUserID)
                .ForeignKey("Security.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateTable(
                "World.CountryRoleAdministration",
                c => new
                    {
                        CountryRoleAdministrationID = c.Long(nullable: false, identity: true),
                        CountryRoleID = c.Long(nullable: false),
                        CanUpdatePermissions = c.Boolean(nullable: false),
                        CanAddPages = c.Boolean(nullable: false),
                        CanDeletePages = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CountryRoleAdministrationID)
                .ForeignKey("World.CydonRole", t => t.CountryRoleID, cascadeDelete: true)
                .Index(t => t.CountryRoleID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("World.CountryRoleAdministration", "CountryRoleID", "World.CydonRole");
            DropForeignKey("Security.SitePermissionUser", "UserID", "Security.User");
            DropForeignKey("World.CountryRoleUser", "UserID", "Security.User");
            DropForeignKey("World.CountryRoleUser", "CountryRoleID", "World.CydonRole");
            DropForeignKey("World.CountryRolePageElement", "PageElementID", "CySys.PageElement");
            DropForeignKey("CySys.PageElement", "PageID", "CySys.Page");
            DropForeignKey("CySys.Navigation", "PageID", "CySys.Page");
            DropForeignKey("CySys.Navigation", "ParentNavigation_NavigationID", "CySys.Navigation");
            DropForeignKey("CySys.Page", "CountryID", "World.Country");
            DropForeignKey("World.CountryRolePageElement", "CountryRoleID", "World.CydonRole");
            DropForeignKey("World.CydonRole", "CountryID", "World.Country");
            DropIndex("World.CountryRoleAdministration", new[] { "CountryRoleID" });
            DropIndex("Security.SitePermissionUser", new[] { "UserID" });
            DropIndex("World.CountryRoleUser", new[] { "UserID" });
            DropIndex("World.CountryRoleUser", new[] { "CountryRoleID" });
            DropIndex("CySys.Navigation", new[] { "ParentNavigation_NavigationID" });
            DropIndex("CySys.Navigation", new[] { "PageID" });
            DropIndex("CySys.Page", new[] { "CountryID" });
            DropIndex("CySys.PageElement", new[] { "PageID" });
            DropIndex("World.CountryRolePageElement", new[] { "PageElementID" });
            DropIndex("World.CountryRolePageElement", new[] { "CountryRoleID" });
            DropIndex("World.CydonRole", new[] { "CountryID" });
            DropTable("World.CountryRoleAdministration");
            DropTable("Security.SitePermissionUser");
            DropTable("World.CountryRoleUser");
            DropTable("CySys.Navigation");
            DropTable("CySys.Page");
            DropTable("CySys.PageElement");
            DropTable("World.CountryRolePageElement");
            DropTable("World.CydonRole");
            DropTable("World.Country");
        }
    }
}
