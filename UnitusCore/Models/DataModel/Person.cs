using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Person : ModelBase
    {
        public Person()
        {
            BelongedCircles = new HashSet<MemberStatus>();
            CommittedProjects = new HashSet<Project>();
            Skills = new HashSet<Skill>();
            InvitedPeople=new HashSet<CircleMemberInvitation>();
        }

        public ApplicationUser ApplicationUser { get; set; }//binded

        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<MemberStatus> BelongedCircles { get; set; }//binded

        public ICollection<Project> CommittedProjects { get; set; }//binded

        public Cource CurrentCource { get; set; }

        public string BelongedColledge { get; set; }

        public string Faculty { get; set; }

        public string Major { get; set; }

        public string Notes { get; set; }

        public ICollection<Event> AttendedEvents { get; set; } //binded

        public ICollection<Skill> Skills { get; set; }//binded

        public ICollection<CircleMemberInvitation> InvitedPeople { get; set; } //binded

        public enum Cource
        {
            UG1, UG2, UG3, UG4, UG5, UG6, MC1, MC2, MC3, MC4, DC1, DC2, DC3, DC4
        }
    }
}