namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationTutorial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "CurrentGrade", c => c.Int(nullable: false));
            DropColumn("dbo.People", "CurrentCource");
        }
        
        public override void Down()
        {
            AddColumn("dbo.People", "CurrentCource", c => c.Int(nullable: false));
            DropColumn("dbo.People", "CurrentGrade");
        }
    }
}