namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserConfigure : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserConfigures",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ShowOwnProfileToOtherCircle = c.Boolean(nullable: false),
                        TargetPerson_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.TargetPerson_Id, cascadeDelete: true)
                .Index(t => t.TargetPerson_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserConfigures", "TargetPerson_Id", "dbo.People");
            DropIndex("dbo.UserConfigures", new[] { "TargetPerson_Id" });
            DropTable("dbo.UserConfigures");
        }
    }
}
