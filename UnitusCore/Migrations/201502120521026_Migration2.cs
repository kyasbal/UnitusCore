namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Migration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CircleMemberInvitations", "EmailAddress", c => c.String());
            AddColumn("dbo.CircleMemberInvitations", "SentDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.CircleMemberInvitations", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CircleMemberInvitations", "Name", c => c.String());
            DropColumn("dbo.CircleMemberInvitations", "SentDate");
            DropColumn("dbo.CircleMemberInvitations", "EmailAddress");
        }
    }
}
