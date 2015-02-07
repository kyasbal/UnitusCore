namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration8 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PasswordResetConfirmations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TargetUserIdentifyCode = c.Guid(nullable: false),
                        ConfirmationId = c.String(),
                        ExpireTime = c.DateTime(nullable: false),
                        UserInfo_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserInfo_Id)
                .Index(t => t.TargetUserIdentifyCode)
                .Index(t => t.UserInfo_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PasswordResetConfirmations", "UserInfo_Id", "dbo.AspNetUsers");
            DropIndex("dbo.PasswordResetConfirmations", new[] { "UserInfo_Id" });
            DropIndex("dbo.PasswordResetConfirmations", new[] { "TargetUserIdentifyCode" });
            DropTable("dbo.PasswordResetConfirmations");
        }
    }
}
