using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels
{
    public class DailyStatisticsJobLog:TableEntity
    {
        public DailyStatisticsJobLog()
        {
            
        }

        public DailyStatisticsJobLog(DailyStatisticJobAction jobType, string argument, double taskTime, string log)
        {
            JobType = jobType;
            ExecutedTime = DateTime.Now;
            RowKey = "h" + ExecutedTime.ToDateCode() + argument.ToHashCode();
            Argument = argument;
            TaskTime = taskTime;
            Log = log;
        }

        public DateTime ExecutedTime { get; set; }

        public double TaskTime { get; set; }

        public string Argument { get; set; }

        public string Log { get; set; }

        [IgnoreProperty]
        public DailyStatisticJobAction JobType
        {
            get { return (DailyStatisticJobAction) Enum.Parse(typeof (DailyStatisticJobAction), PartitionKey); }
            set { PartitionKey = value.ToString(); }
        }
    }

    public enum DailyStatisticJobAction
    {
        SingleUserStatisticGithubContribution,
        SingleUserAchivementStatistics,
        SystemAchivementStatistics
    }
}