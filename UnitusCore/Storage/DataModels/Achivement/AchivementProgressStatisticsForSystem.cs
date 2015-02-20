using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class AchivementProgressStatisticsForSystem:TableEntity
    {
        public AchivementProgressStatisticsForSystem()
        {
            
        }

        public AchivementProgressStatisticsForSystem(string achivementName, int sumPeople, int awardedPeople, double sumPercentage)
        {
            AchivementName = achivementName;
            DateCode = DateTime.Now.ToDateCode();
            SumPeople = sumPeople;
            AwardedPeople = awardedPeople;
            AwardedRate = AwardedPeople/(double)SumPeople;
            SumPercentage = sumPercentage;
            AveragePercentage = sumPercentage/SumPeople;
        }

        public int SumPeople { get; set; }

        public int AwardedPeople { get; set; }

        public double AwardedRate { get; set; }

        public double SumPercentage { get; set; }

        public double AveragePercentage { get; set; }

        [IgnoreProperty]
        public string AchivementName
        {
            get { return PartitionKey; }
            set { PartitionKey = value; }
        }

        [IgnoreProperty]
        public long DateCode
        {
            get { return Convert.ToInt64(RowKey); }
            set { RowKey = value.ToString(); }
        }
    }
}