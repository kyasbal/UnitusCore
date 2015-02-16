namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addStorage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserStatistics", "RecordedTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserStatistics", "RecordedTime");
        }
    }
}
