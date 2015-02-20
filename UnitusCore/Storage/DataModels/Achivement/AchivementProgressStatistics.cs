using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels.Achivement
{
    public class AchivementProgressStatisticsByDay:TableEntity
    {
        public AchivementProgressStatisticsByDay()
        {
            
        }

        public AchivementProgressStatisticsByDay(string uniqueId,double progress)
        {
            Progress = progress;
            UniqueId = uniqueId;
            DateCode = DateTime.Now.ToDateCode();
        }

        public double Progress { get; set; }

        [IgnoreProperty]
        public string UniqueId
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