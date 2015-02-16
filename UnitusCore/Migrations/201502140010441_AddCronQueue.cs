namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCronQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CronQueues",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        QueueTime = c.DateTime(nullable: false),
                        TargetAddress = c.String(),
                        Arguments = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CronQueues");
        }
    }
}
