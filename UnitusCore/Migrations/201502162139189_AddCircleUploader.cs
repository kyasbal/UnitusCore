namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCircleUploader : DbMigration
    {
        public override void Up()
        {
//            CreateTable(
//                "dbo.CircleUploaderEntities",
//                c => new
//                    {
//                        Id = c.Guid(nullable: false),
//                        TargetAddress = c.String(),
//                        LastModefied = c.DateTime(nullable: false),
//                        CreationDate = c.DateTime(nullable: false),
//                        UploadedCircle_Id = c.Guid(nullable: false),
//                        UploadUser_Id = c.String(nullable: false, maxLength: 128),
//                    })
//                .PrimaryKey(t => t.Id)
//                .ForeignKey("dbo.Circles", t => t.UploadedCircle_Id, cascadeDelete: true)
//                .ForeignKey("dbo.AspNetUsers", t => t.UploadUser_Id, cascadeDelete: true)
//                .Index(t => t.UploadedCircle_Id)
//                .Index(t => t.UploadUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CircleUploaderEntities", "UploadUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CircleUploaderEntities", "UploadedCircle_Id", "dbo.Circles");
            DropIndex("dbo.CircleUploaderEntities", new[] { "UploadUser_Id" });
            DropIndex("dbo.CircleUploaderEntities", new[] { "UploadedCircle_Id" });
            DropTable("dbo.CircleUploaderEntities");
        }
    }
}
