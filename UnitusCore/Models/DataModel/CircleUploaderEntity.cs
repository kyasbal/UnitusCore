using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class CircleUploaderEntity:ModelBaseWithTimeLogging
    {
        public ApplicationUser UploadUser { get; set; }

        public Circle UploadedCircle { get; set; }

        public string TargetAddress { get; set; }
    }
}