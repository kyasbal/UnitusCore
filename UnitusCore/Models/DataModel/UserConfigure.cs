using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class UserConfigure:ModelBase
    {
        public Person TargetPerson { get; set; }

        public bool ShowOwnProfileToOtherCircle { get; set; }
    }
}