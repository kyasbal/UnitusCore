namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCronQueueLog : DbMigration
    {
        public override void Up()
        {
//            CreateTable(
//                "dbo.CronQueueLogs",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        ExecutedTime = c.DateTime(nullable: false),
//                        ArgumentLog = c.String(),
//                        WorkedAddress = c.String(),
//                        TakeTime = c.Long(nullable: false),
//                        ResultLog = c.String(),
//                    })
//                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CronQueueLogs");
        }
    }
}
