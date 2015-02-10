﻿using System;
using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Event : ModelBase
    {
        public Event()
        {
            Circles = new HashSet<Circle>();
            Achivements = new HashSet<Achivement>();
            Projects = new HashSet<Project>();
            Participants = new List<Person>();
        }
        public bool AlreadyHosted { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public ICollection<Circle> Circles { get; set; }

        public ICollection<Achivement> Achivements { get; set; }

        public ICollection<Project> Projects { get; set; }

        public ICollection<Person> Participants { get; set; }


    }
}