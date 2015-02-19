using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels
{
    public class ContributeStatisticsByDay:TableEntity
    {
        public ContributeStatisticsByDay()
        {
            
        }

        public static ContributeStatisticsByDay GenerateTodayStatistics(ApplicationUser targetUser)
        {
            return new ContributeStatisticsByDay()
            {
                PartitionKey = targetUser.Id.ToString(),
                RowKey = (DateTime.Now).ToDateCode().ToString(),
                LanguageStatistics = new HashSet<SingleUserLanguageStatisticsByDay>()
            };
        }

        public static ContributeStatisticsByDay GenerateTodayStatistics(string userName)
        {
            return new ContributeStatisticsByDay()
            {
                PartitionKey =userName,
                RowKey = (DateTime.Now).ToDateCode().ToString(),
                LanguageStatistics = new HashSet<SingleUserLanguageStatisticsByDay>()
            };
        }

        public int SumCommit { get; set; }

        public int SumAddition { get; set; }

        public int SumDeletion { get; set; }

        public int SumRepository { get; set; }

        public int SumForking { get; set; }

        public int SumForked { get; set; }

        [IgnoreProperty]
        public HashSet<SingleUserLanguageStatisticsByDay> LanguageStatistics { get; set; }

        [IgnoreProperty]
        public string UniqueId
        {
            get { return PartitionKey + "-" + RowKey; }
        }
    }
}