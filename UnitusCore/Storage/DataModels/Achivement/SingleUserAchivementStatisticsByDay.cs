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

        public SingleUserAchivementStatisticsByDay()
        {
            
        }

        public SingleUserAchivementStatisticsByDay(string achivementId, string userId,bool isAwarded,double currentProgress)
        {
            IsAwarded = isAwarded;
            CurrentProgress = currentProgress;
            PartitionKey = achivementId;
            RowKey = userId;
            if (isAwarded) AwardedDate = DateTime.Now.ToDateCode();
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
    }
}