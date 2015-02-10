using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Achivement : ModelBase
    {
        public Achivement()
        {

        }
        public string Name { get; set; }

        public string Description { get; set; }

        public Event Event { get; set; }//binded

        public Project Project { get; set; }//binded
    }
}