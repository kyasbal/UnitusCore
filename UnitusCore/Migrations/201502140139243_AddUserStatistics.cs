namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserStatistics : DbMigration
    {
        public override void Up()
        {
//            CreateTable(
//                "dbo.UserStatistics",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        RepositoryCount = c.Int(nullable: false),
//                        CommitCount = c.Int(nullable: false),
//                        SumDeletion = c.Int(nullable: false),
//                        SumAddition = c.Int(nullable: false),
//                        LanguageRatioJson = c.String(),
//                    })
//                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserStatistics");
        }
    }
}
