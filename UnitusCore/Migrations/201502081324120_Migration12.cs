namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration12 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetUsers", name: "AccessablePerson_Id", newName: "PersonData_Id");
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_AccessablePerson_Id", newName: "IX_PersonData_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetUsers", name: "IX_PersonData_Id", newName: "IX_AccessablePerson_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "PersonData_Id", newName: "AccessablePerson_Id");
        }
    }
}
