using System;
using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Project : ModelBase
    {
        public Project()
        {
            Circles=new HashSet<Circle>();
            Achivements=new HashSet<Achivement>();
            Members=new HashSet<Person>();
            Events=new HashSet<Event>();
        }
        public string Name { get; set; }

        public string Description { get; set; }

        public string ProjectAddress { get; set; }

        public ICollection<Circle> Circles { get; set; }//binded

        public ICollection<Achivement> Achivements { get; set; }//binded 

        public string Notes { get; set; }

        public ICollection<Person> Members { get; set; }//binded

        public ICollection<Event> Events { get; set; } //binded

        public int Progress { get; set; }

        public DateTime BeginTime { get; set; }

        public enum ProjectType
        {
            EventProject, StandaloneProject
        }
    }
}