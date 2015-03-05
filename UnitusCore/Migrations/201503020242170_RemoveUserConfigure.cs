namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserConfigure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserConfigures", "TargetPerson_Id", "dbo.People");
            DropIndex("dbo.UserConfigures", new[] { "TargetPerson_Id" });
            DropTable("dbo.UserConfigures");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.UserConfigures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ShowOwnProfileToOtherCircle = c.Boolean(nullable: false),
                        TargetPerson_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.UserConfigures", "TargetPerson_Id");
            AddForeignKey("dbo.UserConfigures", "TargetPerson_Id", "dbo.People", "Id", cascadeDelete: true);
        }
    }
}
