using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<UserPermission> Permissions { get; set; }

        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }

        public DbSet<PasswordResetConfirmation> PasswordResetConfirmations { get; set; }

        public DbSet<Circle> Circles { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<Achivement> Achivements { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Statistics> Statisticses { get; set; }

        public DbSet<CircleStatistics> CircleStatisticses { get; set; }

        public DbSet<MemberStatus> MemberStatuses { get; set; }

        public DbSet<CircleMemberInvitation> CircleInvitations { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Circle>().HasMany(c => c.Administrators).WithMany(a => a.AdministrationCircle).Map(
                (m) =>
                {
                }).MapToStoredProcedures();
            modelBuilder.Entity<Circle>().HasMany(c=>c.CircleStatistises).WithRequired(a=>a.RelatedCircle).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<Circle>().HasMany(c=>c.Members).WithRequired(a=>a.TargetCircle).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<MemberStatus>().HasRequired(a=>a.TargetUser).WithMany(a=>a.BelongedCircles).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<Circle>()
                .HasMany(c => c.Events)
                .WithMany(c => c.Circles)
                .Map((f) => { })
                .Map((f) => { })
                .MapToStoredProcedures();
            modelBuilder.Entity<Circle>()
                .HasMany(c => c.Projects)
                .WithMany(a => a.Circles)
                .Map((f) => { })
                .MapToStoredProcedures();
            modelBuilder.Entity<Circle>()
                .HasMany(c => c.MemberInvitations)
                .WithRequired(c => c.InvitedCircle)
                .Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<CircleMemberInvitation>().HasRequired(c=>c.InvitedPerson).WithMany(c=>c.InvitedPeople).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<Project>()
                .HasMany(c => c.Members)
                .WithMany(c => c.CommittedProjects)
                .Map((f) => { })
                .MapToStoredProcedures();
            modelBuilder.Entity<Person>()
                .HasMany(a => a.Skills)
                .WithMany(a => a.People)
                .Map((f) => { })
                .MapToStoredProcedures();
            modelBuilder.Entity<Achivement>().HasOptional(c=>c.Event).WithMany(a=>a.Achivements).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<Achivement>().HasRequired(c=>c.Project).WithMany(a=>a.Achivements).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<EmailConfirmation>().HasRequired(c=>c.TargetUser).WithMany(a=>a.SentConfirmations).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<Project>()
                .HasMany(c => c.Events)
                .WithMany(c => c.Projects)
                .Map((f) => { })
                .MapToStoredProcedures();
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Participants)
                .WithMany(e => e.AttendedEvents)
                .Map((f) => { })
                .MapToStoredProcedures();
            modelBuilder.Entity<PasswordResetConfirmation>().HasRequired(c=>c.TargetUser).WithMany(c=>c.PasswordResetRequests).Map((f)=> {}).WillCascadeOnDelete();
            modelBuilder.Entity<Person>()
                .HasRequired(c => c.ApplicationUser)
                .WithRequiredPrincipal(d => d.PersonData)
                .Map((m) =>
                {
                }
                ).WillCascadeOnDelete();
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.Permissions).WithMany(c => c.AllowedUsers).Map((m) =>
                {
                }).MapToStoredProcedures();
            base.OnModelCreating(modelBuilder);
        }
    }
}