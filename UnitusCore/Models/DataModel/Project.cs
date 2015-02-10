using System;
using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Project : ModelBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ProjectAddress { get; set; }

        public ICollection<Circle> Circles { get; set; }

        public string Notes { get; set; }

        public ICollection<Person> Members { get; set; }

        public int Progress { get; set; }

        public DateTime BeginTime { get; set; }

        public enum ProjectType
        {
            EventProject, StandaloneProject
        }
    }
}