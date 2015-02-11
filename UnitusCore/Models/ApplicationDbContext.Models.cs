using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc.Html;
using Microsoft.AspNet.Identity.EntityFramework;
using UnitusCore.Models.BaseClasses;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Models
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
            Configuration.LazyLoadingEnabled = false;
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

       
    }
}