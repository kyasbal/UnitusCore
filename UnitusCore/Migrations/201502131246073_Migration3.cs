namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Circles", "ActivityDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Circles", "ActivityDate");
        }
    }
}
