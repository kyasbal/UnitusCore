namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SecoundReset : DbMigration
    {
        public override void Up()
        {
            //        CreateTable(
            //"dbo.UserStatistics",
            //c => new
            //{
            //    Id = c.Guid(nullable: false),
            //    RepositoryCount = c.Int(nullable: false),
            //    CommitCount = c.Int(nullable: false),
            //    SumDeletion = c.Int(nullable: false),
            //    SumAddition = c.Int(nullable: false),
            //    LanguageRatioJson = c.String(),
            //    RecordedTime=c.DateTime(nullable:false),
            //    LinkedPerson_UserStatistics_Id=c.Guid(nullable:false)
            //})
            //.PrimaryKey(t => t.Id).ForeignKey("dbo.People",t=>t.LinkedPerson_UserStatistics_Id,cascadeDelete:true)
            //.Index(t=>t.LinkedPerson_UserStatistics_Id);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserStatistics","LinkedPerson_UserStatistics_Id","dbo.People");
            DropIndex("dbo.UserStatistics",new[]{"LinkedPerson_UserStatistics_Id"});
            DropTable("dbo.UserStatistics");
        }
    }
}
