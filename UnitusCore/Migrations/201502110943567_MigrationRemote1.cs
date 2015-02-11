namespace UnitusCore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MigrationRemote1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Achivements",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Circle_Id = c.Guid(),
                        Event_Id = c.Guid(),
                        Project_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Circles", t => t.Circle_Id)
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.Circle_Id)
                .Index(t => t.Event_Id)
                .Index(t => t.Project_Id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AlreadyHosted = c.Boolean(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Circles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        MemberCount = c.Int(nullable: false),
                        WebAddress = c.String(),
                        BelongedSchool = c.String(),
                        Notes = c.String(),
                        Contact = c.String(),
                        CanInterCollege = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        GithubAccessToken = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        PersonData_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.PersonData_Id, cascadeDelete: true)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex")
                .Index(t => t.PersonData_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.PasswordResetConfirmations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        KeyIndex = c.Int(nullable: false),
                        ConfirmationId = c.String(),
                        ExpireTime = c.DateTime(nullable: false),
                        TargetUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TargetUser_Id, cascadeDelete: true)
                .Index(t => t.TargetUser_Id);
            
            CreateTable(
                "dbo.UserPermissions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        PermissionName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.People",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        CurrentCource = c.Int(nullable: false),
                        BelongedColledge = c.String(),
                        Faculty = c.String(),
                        Major = c.String(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberStatus",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Occupation = c.String(),
                        IsActiveMember = c.Boolean(nullable: false),
                        TargetUser_Id = c.Guid(nullable: false),
                        TargetCircle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.TargetUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.Circles", t => t.TargetCircle_Id, cascadeDelete: true)
                .Index(t => t.TargetUser_Id)
                .Index(t => t.TargetCircle_Id);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        ProjectAddress = c.String(),
                        Notes = c.String(),
                        Progress = c.Int(nullable: false),
                        BeginTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CircleMemberInvitations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ConfirmationKey = c.String(),
                        Name = c.String(),
                        InvitedPerson_Id = c.Guid(nullable: false),
                        InvitedCircle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.People", t => t.InvitedPerson_Id, cascadeDelete: true)
                .ForeignKey("dbo.Circles", t => t.InvitedCircle_Id, cascadeDelete: true)
                .Index(t => t.InvitedPerson_Id)
                .Index(t => t.InvitedCircle_Id);
            
            CreateTable(
                "dbo.Skills",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.EmailConfirmations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        KeyIndex = c.Int(nullable: false),
                        ConfirmationId = c.String(),
                        ExpireTime = c.DateTime(nullable: false),
                        TargetUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.TargetUser_Id, cascadeDelete: true)
                .Index(t => t.TargetUser_Id);
            
            CreateTable(
                "dbo.CircleStatistics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GithubUserCount = c.Int(nullable: false),
                        RepositoryCount = c.Int(nullable: false),
                        CommitCount = c.Int(nullable: false),
                        CommitPerUser = c.Double(nullable: false),
                        StatDate = c.DateTime(nullable: false),
                        RelatedCircle_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Circles", t => t.RelatedCircle_Id, cascadeDelete: true)
                .Index(t => t.RelatedCircle_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Statistics",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        StatDate = c.DateTime(nullable: false),
                        SumCircles = c.Int(nullable: false),
                        SumPeoples = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApplicationUserUserPermissions",
                c => new
                    {
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                        UserPermission_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.ApplicationUser_Id, t.UserPermission_Id })
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .ForeignKey("dbo.UserPermissions", t => t.UserPermission_Id, cascadeDelete: true)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.UserPermission_Id);
            
            CreateTable(
                "dbo.ProjectEvents",
                c => new
                    {
                        Project_Id = c.Guid(nullable: false),
                        Event_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.Event_Id })
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.ProjectPersons",
                c => new
                    {
                        Project_Id = c.Guid(nullable: false),
                        Person_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Project_Id, t.Person_Id })
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Project_Id)
                .Index(t => t.Person_Id);
            
            CreateTable(
                "dbo.PersonSkills",
                c => new
                    {
                        Person_Id = c.Guid(nullable: false),
                        Skill_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Person_Id, t.Skill_Id })
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .ForeignKey("dbo.Skills", t => t.Skill_Id, cascadeDelete: true)
                .Index(t => t.Person_Id)
                .Index(t => t.Skill_Id);
            
            CreateTable(
                "dbo.CircleApplicationUsers",
                c => new
                    {
                        Circle_Id = c.Guid(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.Circle_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.Circle_Id)
                .Index(t => t.ApplicationUser_Id);
            
            CreateTable(
                "dbo.CircleEvents",
                c => new
                    {
                        Circle_Id = c.Guid(nullable: false),
                        Event_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Circle_Id, t.Event_Id })
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: true)
                .Index(t => t.Circle_Id)
                .Index(t => t.Event_Id);
            
            CreateTable(
                "dbo.CircleProjects",
                c => new
                    {
                        Circle_Id = c.Guid(nullable: false),
                        Project_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Circle_Id, t.Project_Id })
                .ForeignKey("dbo.Circles", t => t.Circle_Id, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.Project_Id, cascadeDelete: true)
                .Index(t => t.Circle_Id)
                .Index(t => t.Project_Id);
            
            CreateTable(
                "dbo.EventPersons",
                c => new
                    {
                        Event_Id = c.Guid(nullable: false),
                        Person_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Event_Id, t.Person_Id })
                .ForeignKey("dbo.Events", t => t.Event_Id, cascadeDelete: true)
                .ForeignKey("dbo.People", t => t.Person_Id, cascadeDelete: true)
                .Index(t => t.Event_Id)
                .Index(t => t.Person_Id);
            
            CreateStoredProcedure(
                "dbo.ApplicationUserUserPermission_Insert",
                p => new
                    {
                        ApplicationUser_Id = p.String(maxLength: 128),
                        UserPermission_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[ApplicationUserUserPermissions]([ApplicationUser_Id], [UserPermission_Id])
                      VALUES (@ApplicationUser_Id, @UserPermission_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.ApplicationUserUserPermission_Delete",
                p => new
                    {
                        ApplicationUser_Id = p.String(maxLength: 128),
                        UserPermission_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[ApplicationUserUserPermissions]
                      WHERE (([ApplicationUser_Id] = @ApplicationUser_Id) AND ([UserPermission_Id] = @UserPermission_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.ProjectEvent_Insert",
                p => new
                    {
                        Project_Id = p.Guid(),
                        Event_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[ProjectEvents]([Project_Id], [Event_Id])
                      VALUES (@Project_Id, @Event_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.ProjectEvent_Delete",
                p => new
                    {
                        Project_Id = p.Guid(),
                        Event_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[ProjectEvents]
                      WHERE (([Project_Id] = @Project_Id) AND ([Event_Id] = @Event_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.ProjectPerson_Insert",
                p => new
                    {
                        Project_Id = p.Guid(),
                        Person_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[ProjectPersons]([Project_Id], [Person_Id])
                      VALUES (@Project_Id, @Person_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.ProjectPerson_Delete",
                p => new
                    {
                        Project_Id = p.Guid(),
                        Person_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[ProjectPersons]
                      WHERE (([Project_Id] = @Project_Id) AND ([Person_Id] = @Person_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.PersonSkill_Insert",
                p => new
                    {
                        Person_Id = p.Guid(),
                        Skill_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[PersonSkills]([Person_Id], [Skill_Id])
                      VALUES (@Person_Id, @Skill_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.PersonSkill_Delete",
                p => new
                    {
                        Person_Id = p.Guid(),
                        Skill_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[PersonSkills]
                      WHERE (([Person_Id] = @Person_Id) AND ([Skill_Id] = @Skill_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.CircleApplicationUser_Insert",
                p => new
                    {
                        Circle_Id = p.Guid(),
                        ApplicationUser_Id = p.String(maxLength: 128),
                    },
                body:
                    @"INSERT [dbo].[CircleApplicationUsers]([Circle_Id], [ApplicationUser_Id])
                      VALUES (@Circle_Id, @ApplicationUser_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.CircleApplicationUser_Delete",
                p => new
                    {
                        Circle_Id = p.Guid(),
                        ApplicationUser_Id = p.String(maxLength: 128),
                    },
                body:
                    @"DELETE [dbo].[CircleApplicationUsers]
                      WHERE (([Circle_Id] = @Circle_Id) AND ([ApplicationUser_Id] = @ApplicationUser_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.CircleEvent_Insert",
                p => new
                    {
                        Circle_Id = p.Guid(),
                        Event_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[CircleEvents]([Circle_Id], [Event_Id])
                      VALUES (@Circle_Id, @Event_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.CircleEvent_Delete",
                p => new
                    {
                        Circle_Id = p.Guid(),
                        Event_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[CircleEvents]
                      WHERE (([Circle_Id] = @Circle_Id) AND ([Event_Id] = @Event_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.CircleProject_Insert",
                p => new
                    {
                        Circle_Id = p.Guid(),
                        Project_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[CircleProjects]([Circle_Id], [Project_Id])
                      VALUES (@Circle_Id, @Project_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.CircleProject_Delete",
                p => new
                    {
                        Circle_Id = p.Guid(),
                        Project_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[CircleProjects]
                      WHERE (([Circle_Id] = @Circle_Id) AND ([Project_Id] = @Project_Id))"
            );
            
            CreateStoredProcedure(
                "dbo.EventPerson_Insert",
                p => new
                    {
                        Event_Id = p.Guid(),
                        Person_Id = p.Guid(),
                    },
                body:
                    @"INSERT [dbo].[EventPersons]([Event_Id], [Person_Id])
                      VALUES (@Event_Id, @Person_Id)"
            );
            
            CreateStoredProcedure(
                "dbo.EventPerson_Delete",
                p => new
                    {
                        Event_Id = p.Guid(),
                        Person_Id = p.Guid(),
                    },
                body:
                    @"DELETE [dbo].[EventPersons]
                      WHERE (([Event_Id] = @Event_Id) AND ([Person_Id] = @Person_Id))"
            );
            
        }
        
        public override void Down()
        {
            DropStoredProcedure("dbo.EventPerson_Delete");
            DropStoredProcedure("dbo.EventPerson_Insert");
            DropStoredProcedure("dbo.CircleProject_Delete");
            DropStoredProcedure("dbo.CircleProject_Insert");
            DropStoredProcedure("dbo.CircleEvent_Delete");
            DropStoredProcedure("dbo.CircleEvent_Insert");
            DropStoredProcedure("dbo.CircleApplicationUser_Delete");
            DropStoredProcedure("dbo.CircleApplicationUser_Insert");
            DropStoredProcedure("dbo.PersonSkill_Delete");
            DropStoredProcedure("dbo.PersonSkill_Insert");
            DropStoredProcedure("dbo.ProjectPerson_Delete");
            DropStoredProcedure("dbo.ProjectPerson_Insert");
            DropStoredProcedure("dbo.ProjectEvent_Delete");
            DropStoredProcedure("dbo.ProjectEvent_Insert");
            DropStoredProcedure("dbo.ApplicationUserUserPermission_Delete");
            DropStoredProcedure("dbo.ApplicationUserUserPermission_Insert");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Achivements", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.Achivements", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.EventPersons", "Person_Id", "dbo.People");
            DropForeignKey("dbo.EventPersons", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.CircleProjects", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.CircleProjects", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.MemberStatus", "TargetCircle_Id", "dbo.Circles");
            DropForeignKey("dbo.CircleMemberInvitations", "InvitedCircle_Id", "dbo.Circles");
            DropForeignKey("dbo.CircleEvents", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.CircleEvents", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.CircleStatistics", "RelatedCircle_Id", "dbo.Circles");
            DropForeignKey("dbo.CircleApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.CircleApplicationUsers", "Circle_Id", "dbo.Circles");
            DropForeignKey("dbo.EmailConfirmations", "TargetUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.PersonSkills", "Skill_Id", "dbo.Skills");
            DropForeignKey("dbo.PersonSkills", "Person_Id", "dbo.People");
            DropForeignKey("dbo.CircleMemberInvitations", "InvitedPerson_Id", "dbo.People");
            DropForeignKey("dbo.ProjectPersons", "Person_Id", "dbo.People");
            DropForeignKey("dbo.ProjectPersons", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.ProjectEvents", "Event_Id", "dbo.Events");
            DropForeignKey("dbo.ProjectEvents", "Project_Id", "dbo.Projects");
            DropForeignKey("dbo.MemberStatus", "TargetUser_Id", "dbo.People");
            DropForeignKey("dbo.AspNetUsers", "PersonData_Id", "dbo.People");
            DropForeignKey("dbo.ApplicationUserUserPermissions", "UserPermission_Id", "dbo.UserPermissions");
            DropForeignKey("dbo.ApplicationUserUserPermissions", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.PasswordResetConfirmations", "TargetUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Achivements", "Circle_Id", "dbo.Circles");
            DropIndex("dbo.EventPersons", new[] { "Person_Id" });
            DropIndex("dbo.EventPersons", new[] { "Event_Id" });
            DropIndex("dbo.CircleProjects", new[] { "Project_Id" });
            DropIndex("dbo.CircleProjects", new[] { "Circle_Id" });
            DropIndex("dbo.CircleEvents", new[] { "Event_Id" });
            DropIndex("dbo.CircleEvents", new[] { "Circle_Id" });
            DropIndex("dbo.CircleApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.CircleApplicationUsers", new[] { "Circle_Id" });
            DropIndex("dbo.PersonSkills", new[] { "Skill_Id" });
            DropIndex("dbo.PersonSkills", new[] { "Person_Id" });
            DropIndex("dbo.ProjectPersons", new[] { "Person_Id" });
            DropIndex("dbo.ProjectPersons", new[] { "Project_Id" });
            DropIndex("dbo.ProjectEvents", new[] { "Event_Id" });
            DropIndex("dbo.ProjectEvents", new[] { "Project_Id" });
            DropIndex("dbo.ApplicationUserUserPermissions", new[] { "UserPermission_Id" });
            DropIndex("dbo.ApplicationUserUserPermissions", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.CircleStatistics", new[] { "RelatedCircle_Id" });
            DropIndex("dbo.EmailConfirmations", new[] { "TargetUser_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.CircleMemberInvitations", new[] { "InvitedCircle_Id" });
            DropIndex("dbo.CircleMemberInvitations", new[] { "InvitedPerson_Id" });
            DropIndex("dbo.MemberStatus", new[] { "TargetCircle_Id" });
            DropIndex("dbo.MemberStatus", new[] { "TargetUser_Id" });
            DropIndex("dbo.PasswordResetConfirmations", new[] { "TargetUser_Id" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", new[] { "PersonData_Id" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Achivements", new[] { "Project_Id" });
            DropIndex("dbo.Achivements", new[] { "Event_Id" });
            DropIndex("dbo.Achivements", new[] { "Circle_Id" });
            DropTable("dbo.EventPersons");
            DropTable("dbo.CircleProjects");
            DropTable("dbo.CircleEvents");
            DropTable("dbo.CircleApplicationUsers");
            DropTable("dbo.PersonSkills");
            DropTable("dbo.ProjectPersons");
            DropTable("dbo.ProjectEvents");
            DropTable("dbo.ApplicationUserUserPermissions");
            DropTable("dbo.Statistics");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.CircleStatistics");
            DropTable("dbo.EmailConfirmations");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Skills");
            DropTable("dbo.CircleMemberInvitations");
            DropTable("dbo.Projects");
            DropTable("dbo.MemberStatus");
            DropTable("dbo.People");
            DropTable("dbo.UserPermissions");
            DropTable("dbo.PasswordResetConfirmations");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Circles");
            DropTable("dbo.Events");
            DropTable("dbo.Achivements");
        }
    }
}
