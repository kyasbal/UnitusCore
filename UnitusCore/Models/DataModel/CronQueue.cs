using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class CronQueue:ModelBase
    {
        public CronQueue()
        {
            
        }

        public DateTime QueueTime { get; set; }

        public string TargetAddress { get; set; }

        public string Arguments { get; set; }
    }
}