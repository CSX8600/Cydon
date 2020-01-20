namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveCountryRoleAdministrationstable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("World.CountryRoleAdministration", "CountryRoleID", "World.CydonRole");
            DropIndex("World.CountryRoleAdministration", new[] { "CountryRoleID" });
            AddColumn("World.CydonRole", "CanUpdatePermissions", c => c.Boolean(nullable: false));
            AddColumn("World.CydonRole", "CanAddPages", c => c.Boolean(nullable: false));
            AddColumn("World.CydonRole", "CanDeletePages", c => c.Boolean(nullable: false));
            DropTable("World.CountryRoleAdministration");
        }
        
        public override void Down()
        {
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
                .PrimaryKey(t => t.CountryRoleAdministrationID);
            
            DropColumn("World.CydonRole", "CanDeletePages");
            DropColumn("World.CydonRole", "CanAddPages");
            DropColumn("World.CydonRole", "CanUpdatePermissions");
            CreateIndex("World.CountryRoleAdministration", "CountryRoleID");
            AddForeignKey("World.CountryRoleAdministration", "CountryRoleID", "World.CydonRole", "CountryRoleID", cascadeDelete: true);
        }
    }
}
