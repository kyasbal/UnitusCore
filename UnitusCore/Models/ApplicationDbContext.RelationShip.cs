using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Web;
using UnitusCore.Controllers;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Models
{
    public partial class ApplicationDbContext
    {

        private void NoAction(ForeignKeyAssociationMappingConfiguration e)
        { }

        private void NoAction(ManyToManyAssociationMappingConfiguration e)
        { }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Circle>().HasMany(c => c.Administrators).WithMany(a => a.AdministrationCircle).Map(
                (m) =>
                {
                }).MapToStoredProcedures();
            modelBuilder.Entity<Circle>().HasMany(c => c.CircleStatistises).WithRequired(a => a.RelatedCircle).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Circle>().HasMany(c => c.Members).WithRequired(a => a.TargetCircle).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<MemberStatus>().HasRequired(a => a.TargetUser).WithMany(a => a.BelongedCircles).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Circle>()
                .HasMany(c => c.Events)
                .WithMany(c => c.Circles)
                .Map(NoAction)
                .MapToStoredProcedures();
            modelBuilder.Entity<Circle>()
                .HasMany(c => c.Projects)
                .WithMany(a => a.Circles)
                .Map(NoAction)
                .MapToStoredProcedures();
            modelBuilder.Entity<Circle>()
                .HasMany(c => c.MemberInvitations)
                .WithRequired(c => c.InvitedCircle)
                .Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<CircleMemberInvitation>().HasRequired(c => c.InvitedPerson).WithMany(c => c.InvitedPeople).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Project>()
                .HasMany(c => c.Members)
                .WithMany(c => c.CommittedProjects)
                .Map(NoAction)
                .MapToStoredProcedures();
            modelBuilder.Entity<Person>()
                .HasMany(a => a.Skills)
                .WithMany(a => a.People)
                .Map(NoAction)
                .MapToStoredProcedures();
            modelBuilder.Entity<Achivement>().HasOptional(c => c.Event).WithMany(a => a.Achivements).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Achivement>().HasRequired(c => c.Project).WithMany(a => a.Achivements).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<EmailConfirmation>().HasRequired(c => c.TargetUser).WithMany(a => a.SentConfirmations).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Project>()
                .HasMany(c => c.Events)
                .WithMany(c => c.Projects)
                .Map(NoAction)
                .MapToStoredProcedures();
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Participants)
                .WithMany(e => e.AttendedEvents)
                .Map(NoAction)
                .MapToStoredProcedures();
            modelBuilder.Entity<PasswordResetConfirmation>().HasRequired(c => c.TargetUser).WithMany(c => c.PasswordResetRequests).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Person>()
                .HasRequired(c => c.ApplicationUser)
                .WithRequiredPrincipal(d => d.PersonData)
                .Map(NoAction
                ).WillCascadeOnDelete();
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.Permissions).WithMany(c => c.AllowedUsers).Map(NoAction).MapToStoredProcedures();
            modelBuilder.Entity<UserStatistics>()
                .HasRequired(a => a.LinkedPerson)
                .WithMany(a => a.UserStatistics)
                .Map(NoAction)
                .WillCascadeOnDelete();
            modelBuilder.Entity<CircleUploaderEntity>().HasRequired(a=>a.UploadUser).WithMany(a=>a.UploadedEntities).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<CircleUploaderEntity>().HasRequired(a=>a.UploadedCircle).WithMany(a=>a.UploadedEntities).Map(NoAction).WillCascadeOnDelete();
            modelBuilder.Entity<Person>().HasOptional(a=>a.UserConfigure).WithRequired(a=>a.TargetPerson).Map(NoAction).WillCascadeOnDelete();
            base.OnModelCreating(modelBuilder);
        }
    }
}