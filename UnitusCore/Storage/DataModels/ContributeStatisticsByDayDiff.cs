using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCore.Storage.DataModels
{
    public class ContributeStatisticsByDayDiff:TableEntity
    {
        private static int GetDiff(Func<ContributeStatisticsByDay, int> f, ContributeStatisticsByDay today,
            ContributeStatisticsByDay last)
        {
            return f(today) - f(last);
        }

        public ContributeStatisticsByDayDiff()
        {
            
        }

        public static ContributeStatisticsByDayDiff GenerateTodayDiff(string userId,ContributeStatisticsByDay today,ContributeStatisticsByDay last)
        {
            ContributeStatisticsByDayDiff diff=new ContributeStatisticsByDayDiff()
            {
                PartitionKey = userId,
                RowKey = DateTime.Now.ToDateCode().ToString(),
                SumCommitDiff =GetDiff(c=>c.SumCommit,today,last),
                SumAdditionDiff = GetDiff(c=>c.SumAddition,today,last),
                SumDeletionDiff = GetDiff(c=>c.SumDeletion,today,last),
                SumRepositoryDiff = GetDiff(c=>c.SumRepository,today,last),
                LanguageStatisticsDiffs = new HashSet<SingleUserLanguageStatisticsByDayDiff>()
            };
            foreach (var stats in today.LanguageStatistics)
            {
                var lastEnt = last.LanguageStatistics.FirstOrDefault(a => a.Language.Equals(stats.Language));
                if (lastEnt != null)
                {
                    diff.LanguageStatisticsDiffs.Add(new SingleUserLanguageStatisticsByDayDiff(diff.UniqueId, stats,
                        lastEnt));
                }
            }
            return diff;
        }

        public int SumCommitDiff { get; set; }

        public int SumAdditionDiff { get; set; }

        public int SumDeletionDiff { get; set; }

        public int SumRepositoryDiff { get; set; }

        [IgnoreProperty]
        public HashSet<SingleUserLanguageStatisticsByDayDiff> LanguageStatisticsDiffs { get; set; }

        [IgnoreProperty]
        public string UniqueId
        {
            get { return PartitionKey + "-" + RowKey; }
        }
    }
}