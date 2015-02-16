using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class CronQueueLog:ModelBase
    {
        public CronQueueLog()
        {
            
        }

        public CronQueueLog(DateTime executedTime, string argumentLog, string workedAddress, long takeTime, string resultLog)
        {
            ExecutedTime = executedTime;
            ArgumentLog = argumentLog;
            WorkedAddress = workedAddress;
            TakeTime = takeTime;
            ResultLog = resultLog;
            GenerateId();
        }

        public DateTime ExecutedTime { get; set; }

        public string ArgumentLog { get; set; }

        public string WorkedAddress { get; set; }

        public long TakeTime { get; set; }

        public string ResultLog { get; set; }
    }
}