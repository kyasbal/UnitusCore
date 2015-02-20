using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class SingleUserAchivementStatisticsByDay:TableEntity
    {
        public bool IsAwarded { get; set; }

        public double CurrentProgress { get; set; }

        public long AwardedDate { get; set; }

        public double ProgressDiff { get; set; }

        public SingleUserAchivementStatisticsByDay()
        {
            ProgressRecords=new HashSet<AchivementProgressStatisticsByDay>();
        }

        public SingleUserAchivementStatisticsByDay(string achivementId, string userId,bool isAwarded,double currentProgress,double progressDiff)
        {
            IsAwarded = isAwarded;
            CurrentProgress = currentProgress;
            PartitionKey = achivementId;
            RowKey = userId;
            ProgressDiff = progressDiff;
            if (isAwarded) AwardedDate = DateTime.Now.ToDateCode();
            ProgressRecords=new HashSet<AchivementProgressStatisticsByDay>();
        }

        [IgnoreProperty]
        public string UniqueId
        {
            get { return PartitionKey + "-" + RowKey; }
        }

        [IgnoreProperty]
        public string UserId
        {
            get { return RowKey; }
        }

        [IgnoreProperty]
        public string AchivementId
        {
            get{return PartitionKey;}
        }

        [IgnoreProperty]
        public HashSet<AchivementProgressStatisticsByDay> ProgressRecords { get; set; } 
    }
}