using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Skill : ModelBase
    {
        public string Name { get; set; }

        public ICollection<Person> People { get; set; }
    }
}