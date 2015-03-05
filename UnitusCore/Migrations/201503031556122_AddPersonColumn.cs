namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPersonColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.People", "NickName", c => c.String());
            AddColumn("dbo.People", "Url", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.People", "Url");
            DropColumn("dbo.People", "NickName");
        }
    }
}
