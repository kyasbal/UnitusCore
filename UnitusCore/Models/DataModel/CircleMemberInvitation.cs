using System;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class CircleMemberInvitation : ModelBase
    {
        public Circle InvitedCircle { get; set; }//binded

        public Person InvitedPerson { get; set; }//binded

        public string ConfirmationKey { get; set; }

        public string Name { get; set; }
    }
}