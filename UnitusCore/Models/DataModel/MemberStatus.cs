using System;
using System.ComponentModel.DataAnnotations.Schema;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class MemberStatus : ModelBase
    {
        public string Occupation { get; set; }

        public bool IsActiveMember { get; set; }
        [Index]
        public Guid TargertUserKey { get; set; }
    }
}