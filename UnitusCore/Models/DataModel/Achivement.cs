using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Achivement : ModelBase
    {
        public Achivement()
        {
            Events = new HashSet<Event>();
            Projects = new HashSet<Project>();
        }
        public string Name { get; set; }

        public string Description { get; set; }

        public ICollection<Event> Events { get; set; }

        public ICollection<Project> Projects { get; set; }
    }
}