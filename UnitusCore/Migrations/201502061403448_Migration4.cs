namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PermissionName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserUserPermissions",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        UserPermission_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.UserPermission_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserPermissions", t => t.UserPermission_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.UserPermission_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserUserPermissions", "UserPermission_Id", "dbo.UserPermissions");
            DropForeignKey("dbo.ApplicationUserUserPermissions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.ApplicationUserUserPermissions", new[] { "UserPermission_Id" });
            DropIndex("dbo.ApplicationUserUserPermissions", new[] { "ApplicationUser_Id" });
            DropTable("dbo.ApplicationUserUserPermissions");
            DropTable("dbo.UserPermissions");
        }
    }
}
