using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Circle : ModelBase
    {
        public Circle()
        {
            Events = new HashSet<Event>();
            Projects = new HashSet<Project>();
            Achivements = new HashSet<Achivement>();
            Members = new HashSet<MemberStatus>();
            CircleStatistises = new HashSet<CircleStatistics>();
            Administrators=new HashSet<IdentityUser>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MemberCount { get; set; }

        public string WebAddress { get; set; }

        public string BelongedSchool { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<Project> Projects { get; set; }

        public ICollection<Achivement> Achivements { get; set; }

        public ICollection<MemberStatus> Members { get; set; }

        public string Notes { get; set; }

        public string Contact { get; set; }

        public bool CanInterCollege { get; set; }

        public ICollection<CircleStatistics> CircleStatistises { get; set; }

        public CircleStatistics LastCircleStatistics { get; set; }

        public ICollection<IdentityUser>  Administrators { get; set; }
    }
}