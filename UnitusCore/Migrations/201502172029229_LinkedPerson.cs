namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkedPerson : DbMigration
    {
        public override void Up()
        {
            //RenameColumn(table: "dbo.UserStatistics", name: "LinkedPerson_Id", newName: "LinkedPerson_UserStatistics_Id");
            //RenameIndex(table: "dbo.UserStatistics", name: "IX_LinkedPerson_Id", newName: "IX_LinkedPerson_UserStatistics_Id");
        }
        
        public override void Down()
        {
        //    RenameIndex(table: "dbo.UserStatistics", name: "IX_LinkedPerson_UserStatistics_Id", newName: "IX_LinkedPerson_Id");
        //    RenameColumn(table: "dbo.UserStatistics", name: "LinkedPerson_UserStatistics_Id", newName: "LinkedPerson_Id");
        }
    }
}
