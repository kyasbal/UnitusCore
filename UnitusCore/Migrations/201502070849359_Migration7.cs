namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EmailConfirmations", "TargetUserIdentifyCode", c => c.Guid(nullable: false));
            CreateIndex("dbo.EmailConfirmations", "TargetUserIdentifyCode");
        }
        
        public override void Down()
        {
            DropIndex("dbo.EmailConfirmations", new[] { "TargetUserIdentifyCode" });
            DropColumn("dbo.EmailConfirmations", "TargetUserIdentifyCode");
        }
    }
}
