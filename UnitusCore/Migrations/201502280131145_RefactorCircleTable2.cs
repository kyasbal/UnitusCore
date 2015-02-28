namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RefactorCircleTable2 : DbMigration
    {
        public override void Up()
        {
//            AddColumn("dbo.Circles", "CanInterColledge", c => c.Boolean(nullable: false));
//            DropColumn("dbo.Circles", "CanInterCollege");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Circles", "CanInterCollege", c => c.Boolean(nullable: false));
            DropColumn("dbo.Circles", "CanInterColledge");
        }
    }
}
