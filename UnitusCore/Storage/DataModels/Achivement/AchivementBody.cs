using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class AchivementBody:TableEntity
    {
        public AchivementBody()
        {
            
        }

        public AchivementBody(string achivementName)
        {
            PartitionKey = "AchivementBody";
            AchivementName = achivementName;
            AwardedCount = 0;
            AwardedRate = 0;
        }

        public string AchivementDescription { get; set; }

        public int AwardedCount { get; set; }

        public double AwardedRate { get; set; }

        public int SumPeople { get; set; }

        public double AvarageProgress { get; set; }

        [IgnoreProperty]
        public string AchivementName
        {
            get { return RowKey; }
            set { RowKey = value; }
        }

    }
}