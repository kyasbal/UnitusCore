using System;
using System.ComponentModel.DataAnnotations.Schema;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class MemberStatus : ModelBase
    {
        public string Occupation { get; set; }

        public bool IsActiveMember { get; set; }

        public Person TargetUser { get; set; }//binded

        public Circle TargetCircle { get; set; }//binded
    }
}