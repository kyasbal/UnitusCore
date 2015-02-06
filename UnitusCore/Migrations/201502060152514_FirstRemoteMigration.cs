namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstRemoteMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achivements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Circle_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Circles", t => t.Circle_Id)
                .Index(t => t.Circle_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AlreadyHosted = c.Boolean(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Circles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        MemberCount = c.Int(nullable: false),
                        WebAddress = c.String(),
                        BelongedSchool = c.String(),
                        Notes = c.String(),
                        Contact = c.String(),
                        CanInterCollege = c.Boolean(nullable: false),
                        Person_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.Person_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ProjectAddress = c.String(),
                        Notes = c.String(),
                        Progress = c.Int(nullable: false),
                        BeginTime = c.DateTime(nullable: false),
                        Event_Id = c.Guid(),
                        Achivement_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .ForeignKey("dbo.Achivements", t => t.Achivement_Id)
                .Index(t => t.Event_Id)
                .Index(t => t.Achivement_Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        CurrentCource = c.Int(nullable: false),
                        BelongedColledge = c.String(),
                        Faculty = c.String(),
                        Major = c.String(),
                        Notes = c.String(),
                        Event_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Events", t => t.Event_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Statistics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StatDate = c.DateTime(nullable: false),
                        SumCircles = c.Int(nullable: false),
                        SumPeoples = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EventAchivements",
                c => new
                    {
                        Event_Id = c.Guid(nullable: false),
                        Achivement_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_Id, t.Achivement_Id })
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: true)
                .ForeignKey("dbo.Achivements", t => t.Achivement_Id, cascadeDelete: true)
                .Index(t => t.Event_Id)
                .Index(t => t.Achivement_Id);
            
            CreateTable(
                "dbo.CircleEvents",
                c => new
                    {
                        Circle_Id = c.Guid(nullable: false),
                        Event_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Circle_Id, t.Event_Id })
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: true)
                .Index(t => t.Circle_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.ProjectCircles",
                c => new
                    {
                        Project_Id = c.Guid(nullable: false),
                        Circle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.Circle_Id })
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Circle_Id);
            
            CreateTable(
                "dbo.PersonProjects",
                c => new
                    {
                        Person_Id = c.Guid(nullable: false),
                        Project_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Project_Id })
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.Person_Id)
                .Index(t => t.Project_Id);
            
            CreateTable(
                "dbo.SkillPersons",
                c => new
                    {
                        Skill_Id = c.Guid(nullable: false),
                        Person_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Skill_Id, t.Person_Id })
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Skill_Id)
                .Index(t => t.Person_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "Achivement_Id", "dbo.Achivements");
            DropForeignKey("dbo.Projects", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.People", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.SkillPersons", "Person_Id", "dbo.People");
            DropForeignKey("dbo.SkillPersons", "Skill_Id", "dbo.Skills");
            DropForeignKey("dbo.PersonProjects", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.PersonProjects", "Person_Id", "dbo.People");
            DropForeignKey("dbo.Circles", "Person_Id", "dbo.People");
            DropForeignKey("dbo.ProjectCircles", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.ProjectCircles", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.CircleEvents", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.CircleEvents", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.Achivements", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.EventAchivements", "Achivement_Id", "dbo.Achivements");
            DropForeignKey("dbo.EventAchivements", "Event_Id", "dbo.Events");
            DropIndex("dbo.SkillPersons", new[] { "Person_Id" });
            DropIndex("dbo.SkillPersons", new[] { "Skill_Id" });
            DropIndex("dbo.PersonProjects", new[] { "Project_Id" });
            DropIndex("dbo.PersonProjects", new[] { "Person_Id" });
            DropIndex("dbo.ProjectCircles", new[] { "Circle_Id" });
            DropIndex("dbo.ProjectCircles", new[] { "Project_Id" });
            DropIndex("dbo.CircleEvents", new[] { "Event_Id" });
            DropIndex("dbo.CircleEvents", new[] { "Circle_Id" });
            DropIndex("dbo.EventAchivements", new[] { "Achivement_Id" });
            DropIndex("dbo.EventAchivements", new[] { "Event_Id" });
            DropIndex("dbo.People", new[] { "Event_Id" });
            DropIndex("dbo.Projects", new[] { "Achivement_Id" });
            DropIndex("dbo.Projects", new[] { "Event_Id" });
            DropIndex("dbo.Circles", new[] { "Person_Id" });
            DropIndex("dbo.Achivements", new[] { "Circle_Id" });
            DropTable("dbo.SkillPersons");
            DropTable("dbo.PersonProjects");
            DropTable("dbo.ProjectCircles");
            DropTable("dbo.CircleEvents");
            DropTable("dbo.EventAchivements");
            DropTable("dbo.Statistics");
            DropTable("dbo.Skills");
            DropTable("dbo.People");
            DropTable("dbo.Projects");
            DropTable("dbo.Circles");
            DropTable("dbo.Events");
            DropTable("dbo.Achivements");
        }
    }
}
