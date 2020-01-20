namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisplayOrdertoPageElement : DbMigration
    {
        public override void Up()
        {
            AddColumn("CySys.PageElement", "DisplayOrder", c => c.Byte(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("CySys.PageElement", "DisplayOrder");
        }
    }
}
