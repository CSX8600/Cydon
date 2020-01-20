namespace Cydon.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecurityObjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Security.Session",
                c => new
                    {
                        SessionID = c.Long(nullable: false, identity: true),
                        SessionStateID = c.String(nullable: false),
                        UserID = c.Long(nullable: false),
                        Expiration = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SessionID);
            
            CreateTable(
                "Security.UserDiscord",
                c => new
                    {
                        UserDiscordID = c.Long(nullable: false, identity: true),
                        UserID = c.Long(),
                        AccessToken = c.String(),
                        RefreshToken = c.String(),
                        Expiration = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserDiscordID)
                .ForeignKey("Security.User", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "Security.User",
                c => new
                    {
                        UserID = c.Long(nullable: false, identity: true),
                        Username = c.String(maxLength: 30),
                        Password = c.Binary(),
                        IsDiscordUser = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("Security.UserDiscord", "UserID", "Security.User");
            DropIndex("Security.UserDiscord", new[] { "UserID" });
            DropTable("Security.User");
            DropTable("Security.UserDiscord");
            DropTable("Security.Session");
        }
    }
}
