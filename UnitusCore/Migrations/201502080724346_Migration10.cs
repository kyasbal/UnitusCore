namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration10 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Circles", "Person_Id", "dbo.People");
            DropIndex("dbo.Circles", new[] { "Person_Id" });
            CreateTable(
                "dbo.CircleStatistics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GithubUserCount = c.Int(nullable: false),
                        RepositoryCount = c.Int(nullable: false),
                        CommitCount = c.Int(nullable: false),
                        CommitPerUser = c.Double(nullable: false),
                        StatDate = c.DateTime(nullable: false),
                        RelatedCircle_Id = c.Guid(),
                        Circle_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Circles", t => t.RelatedCircle_Id)
                .ForeignKey("dbo.Circles", t => t.Circle_Id)
                .Index(t => t.RelatedCircle_Id)
                .Index(t => t.Circle_Id);
            
            CreateTable(
                "dbo.PersonCircles",
                c => new
                    {
                        Person_Id = c.Guid(nullable: false),
                        Circle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Circle_Id })
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .Index(t => t.Person_Id)
                .Index(t => t.Circle_Id);
            
            AddColumn("dbo.Circles", "LastCircleStatistics_Id", c => c.Guid());
            CreateIndex("dbo.Circles", "LastCircleStatistics_Id");
            AddForeignKey("dbo.Circles", "LastCircleStatistics_Id", "dbo.CircleStatistics", "Id");
            DropColumn("dbo.Circles", "Person_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Circles", "Person_Id", c => c.Guid());
            DropForeignKey("dbo.Circles", "LastCircleStatistics_Id", "dbo.CircleStatistics");
            DropForeignKey("dbo.CircleStatistics", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.CircleStatistics", "RelatedCircle_Id", "dbo.Circles");
            DropForeignKey("dbo.PersonCircles", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.PersonCircles", "Person_Id", "dbo.People");
            DropIndex("dbo.PersonCircles", new[] { "Circle_Id" });
            DropIndex("dbo.PersonCircles", new[] { "Person_Id" });
            DropIndex("dbo.CircleStatistics", new[] { "Circle_Id" });
            DropIndex("dbo.CircleStatistics", new[] { "RelatedCircle_Id" });
            DropIndex("dbo.Circles", new[] { "LastCircleStatistics_Id" });
            DropColumn("dbo.Circles", "LastCircleStatistics_Id");
            DropTable("dbo.PersonCircles");
            DropTable("dbo.CircleStatistics");
            CreateIndex("dbo.Circles", "Person_Id");
            AddForeignKey("dbo.Circles", "Person_Id", "dbo.People", "Id");
        }
    }
}
