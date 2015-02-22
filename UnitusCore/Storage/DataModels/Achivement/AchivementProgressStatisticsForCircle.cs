using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Octokit.Helpers;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class AchivementProgressStatisticsForCircle:TableEntity
    {
        public AchivementProgressStatisticsForCircle()
        {
            
        }

        public AchivementProgressStatisticsForCircle(string achivementName,string circleId,double avrProgress, int sumPeople, int awardedCount, double awardedRate)
        {
            AchivementName = achivementName;
            CircleId = circleId;
            AvrProgress = avrProgress;
            SumPeople = sumPeople;
            AwardedCount = awardedCount;
            AwardedRate = awardedRate;
        }

        public double AvrProgress { get; set; }

        public int SumPeople { get; set; }

        public int AwardedCount { get; set; }

        public double AwardedRate { get; set; }

        [IgnoreProperty]
        public string CircleId
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        [IgnoreProperty]
        public  string AchivementName
        {
            get { return RowKey; }
            set { RowKey = value; }
        }
    }

    public class AchivementProgressStatisticsForCircleLog : AchivementProgressStatisticsForCircle
    {
        public AchivementProgressStatisticsForCircleLog()
        {
            
        }

        public AchivementProgressStatisticsForCircleLog(AchivementProgressStatisticsForCircle circleStat) : base(circleStat.AchivementName,circleStat.CircleId,circleStat.AvrProgress,circleStat.SumPeople,circleStat.AwardedCount,circleStat.AwardedRate)
        {
            LoggedTime = DateTime.Today;
            PartitionKey = AchivementName + "-" + CircleId;
            RowKey = LoggedTime.ToUnixTime().ToString();
        }

        public DateTime LoggedTime { get; set; }

        
    }
}