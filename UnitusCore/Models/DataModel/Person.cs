using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Person : ModelBase
    {
        public Person()
        {
            BelongedCircles = new HashSet<Circle>();
            CommittedProjects = new HashSet<Project>();
            Skills = new HashSet<Skill>();
        }
        public string Name { get; set; }

        public string Email { get; set; }

        public ICollection<Circle> BelongedCircles { get; set; }

        public ICollection<Project> CommittedProjects { get; set; }

        public Cource CurrentCource { get; set; }

        public string BelongedColledge { get; set; }

        public string Faculty { get; set; }

        public string Major { get; set; }

        public string Notes { get; set; }

        public ICollection<Skill> Skills { get; set; }



        public enum Cource
        {
            UG1, UG2, UG3, UG4, UG5, UG6, MC1, MC2, MC3, MC4, DC1, DC2, DC3, DC4
        }
    }
}