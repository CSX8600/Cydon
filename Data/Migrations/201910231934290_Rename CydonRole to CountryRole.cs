namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameCydonRoletoCountryRole : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "World.CydonRole", newName: "CountryRole");
        }
        
        public override void Down()
        {
            RenameTable(name: "World.CountryRole", newName: "CydonRole");
        }
    }
}
