namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration3 : DbMigration
    {
        public override void Up()
        {            
            AddColumn("dbo.AspNetUsers", "AccessablePerson_Id", c => c.Guid());
            CreateIndex("dbo.AspNetUsers", "AccessablePerson_Id");
            AddForeignKey("dbo.AspNetUsers", "AccessablePerson_Id", "dbo.People", "Id");
            DropColumn("dbo.AspNetUsers", "AllowAccessName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "AllowAccessName", c => c.String());
            DropForeignKey("dbo.AspNetUsers", "AccessablePerson_Id", "dbo.People");
            DropIndex("dbo.AspNetUsers", new[] { "AccessablePerson_Id" });
            DropColumn("dbo.AspNetUsers", "AccessablePerson_Id");
        }
    }
}
