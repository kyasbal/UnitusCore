namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfirmations", "ConfirmationId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EmailConfirmations", "ConfirmationId");
        }
    }
}
