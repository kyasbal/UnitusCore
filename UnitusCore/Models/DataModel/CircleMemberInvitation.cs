using System;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class CircleMemberInvitation : ModelBase
    {
        public Guid TargetCircleId { get; set; }

        public string ConfirmationKey { get; set; }

        public string Name { get; set; }
    }
}