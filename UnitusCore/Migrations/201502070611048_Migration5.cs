namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration5 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ApplicationUserUserPermissions", newName: "UserPermissionApplicationUsers");
            DropPrimaryKey("dbo.UserPermissionApplicationUsers");
            CreateTable(
                "dbo.EmailConfirmations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ExpireTime = c.DateTime(nullable: false),
                        UserInfo_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserInfo_Id)
                .Index(t => t.UserInfo_Id);
            
            AddPrimaryKey("dbo.UserPermissionApplicationUsers", new[] { "UserPermission_Id", "ApplicationUser_Id" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmailConfirmations", "UserInfo_Id", "dbo.AspNetUsers");
            DropIndex("dbo.EmailConfirmations", new[] { "UserInfo_Id" });
            DropPrimaryKey("dbo.UserPermissionApplicationUsers");
            DropTable("dbo.EmailConfirmations");
            AddPrimaryKey("dbo.UserPermissionApplicationUsers", new[] { "ApplicationUser_Id", "UserPermission_Id" });
            RenameTable(name: "dbo.UserPermissionApplicationUsers", newName: "ApplicationUserUserPermissions");
        }
    }
}
