namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration1 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.PersonProjects", newName: "ProjectPersons");
            RenameTable(name: "dbo.UserPermissionApplicationUsers", newName: "ApplicationUserUserPermissions");
            DropIndex("dbo.AspNetUsers", new[] { "PersonData_Id" });
            DropPrimaryKey("dbo.ProjectPersons");
            DropPrimaryKey("dbo.ApplicationUserUserPermissions");
            CreateTable(
                "dbo.CircleMemberInvitations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        TargetCircleId = c.Guid(nullable: false),
                        ConfirmationKey = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CircleApplicationUsers",
                c => new
                    {
                        Circle_Id = c.Guid(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Circle_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Circle_Id)
                .Index(t => t.ApplicationUser_Id);
            
            AlterColumn("dbo.AspNetUsers", "PersonData_Id", c => c.Guid(nullable: false));
            AddPrimaryKey("dbo.ProjectPersons", new[] { "Project_Id", "Person_Id" });
            AddPrimaryKey("dbo.ApplicationUserUserPermissions", new[] { "ApplicationUser_Id", "UserPermission_Id" });
            CreateIndex("dbo.AspNetUsers", "PersonData_Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CircleApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CircleApplicationUsers", "Circle_Id", "dbo.Circles");
            DropIndex("dbo.CircleApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.CircleApplicationUsers", new[] { "Circle_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "PersonData_Id" });
            DropPrimaryKey("dbo.ApplicationUserUserPermissions");
            DropPrimaryKey("dbo.ProjectPersons");
            AlterColumn("dbo.AspNetUsers", "PersonData_Id", c => c.Guid());
            DropTable("dbo.CircleApplicationUsers");
            DropTable("dbo.CircleMemberInvitations");
            AddPrimaryKey("dbo.ApplicationUserUserPermissions", new[] { "UserPermission_Id", "ApplicationUser_Id" });
            AddPrimaryKey("dbo.ProjectPersons", new[] { "Person_Id", "Project_Id" });
            CreateIndex("dbo.AspNetUsers", "PersonData_Id");
            RenameTable(name: "dbo.ApplicationUserUserPermissions", newName: "UserPermissionApplicationUsers");
            RenameTable(name: "dbo.ProjectPersons", newName: "PersonProjects");
        }
    }
}
