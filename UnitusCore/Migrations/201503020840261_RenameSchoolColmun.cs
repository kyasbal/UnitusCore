namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSchoolColmun : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.People","BelongedColledge","BelongedSchool");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.People", "BelongedSchool","BelongedColledge");
        }
    }
}
