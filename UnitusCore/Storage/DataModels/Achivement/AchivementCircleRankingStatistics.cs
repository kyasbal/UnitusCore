using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class AchivementCircleRankingStatistics:TableEntity
    {
        public AchivementCircleRankingStatistics()
        {
            
        }

        public AchivementCircleRankingStatistics(string circleId,string userId,string achivementId,int rank)
        {
            PartitionKey = achivementId + "-" + circleId;
            RowKey = userId;
            Rank = rank;
        }

        public int Rank { get; set; }
    }
}