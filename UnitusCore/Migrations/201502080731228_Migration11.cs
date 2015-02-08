namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PersonCircles", "Person_Id", "dbo.People");
            DropForeignKey("dbo.PersonCircles", "Circle_Id", "dbo.Circles");
            DropIndex("dbo.PersonCircles", new[] { "Person_Id" });
            DropIndex("dbo.PersonCircles", new[] { "Circle_Id" });
            CreateTable(
                "dbo.MemberStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Occupation = c.String(),
                        IsActiveMember = c.Boolean(nullable: false),
                        TargetPerson_Id = c.Guid(),
                        Circle_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.TargetPerson_Id)
                .ForeignKey("dbo.Circles", t => t.Circle_Id)
                .Index(t => t.TargetPerson_Id)
                .Index(t => t.Circle_Id);
            
            AddColumn("dbo.Circles", "Person_Id", c => c.Guid());
            CreateIndex("dbo.Circles", "Person_Id");
            AddForeignKey("dbo.Circles", "Person_Id", "dbo.People", "Id");
            DropTable("dbo.PersonCircles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PersonCircles",
                c => new
                    {
                        Person_Id = c.Guid(nullable: false),
                        Circle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Circle_Id });
            
            DropForeignKey("dbo.MemberStatus", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.MemberStatus", "TargetPerson_Id", "dbo.People");
            DropForeignKey("dbo.Circles", "Person_Id", "dbo.People");
            DropIndex("dbo.MemberStatus", new[] { "Circle_Id" });
            DropIndex("dbo.MemberStatus", new[] { "TargetPerson_Id" });
            DropIndex("dbo.Circles", new[] { "Person_Id" });
            DropColumn("dbo.Circles", "Person_Id");
            DropTable("dbo.MemberStatus");
            CreateIndex("dbo.PersonCircles", "Circle_Id");
            CreateIndex("dbo.PersonCircles", "Person_Id");
            AddForeignKey("dbo.PersonCircles", "Circle_Id", "dbo.Circles", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PersonCircles", "Person_Id", "dbo.People", "Id", cascadeDelete: true);
        }
    }
}
