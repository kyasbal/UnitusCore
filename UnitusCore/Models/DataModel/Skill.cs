using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Skill : ModelBase
    {
        [Index]
        public string Name { get; set; }

        public ICollection<Person> People { get; set; }//binded
    }
}