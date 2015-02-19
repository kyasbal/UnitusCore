using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Util;

namespace UnitusCore.Storage
{
    public class ContributeStatisticsByDayStorage
    {
        private const string ContributeStatisticsByDayTableName = "StatisticsContributeByDay";

        private const string SingleUserLanguageStatisticsByDayTableName = "StatisticsSingleUserLanguageByDay";

        private const string ContributeStatisticsByDayDiffTableName = "StatisticsContributeByDayDiff";

        private const string SingleUserLanguageStatisticsByDayDiffTableName = "StatisticsSingleUserLanguageByDayDiff";

        private readonly TableStorageConnection _connection;

        private readonly CloudTable _contributeStatisticsByDayTable;

        private readonly CloudTable _singleUserLanguageStatisticsByDayTable;

        private readonly CloudTable _contributeStatisticsByDayDiffTable;

        private readonly CloudTable _singleUserLanguageStatisticsByDayDiffTable;

        private Dictionary<string,string> ReplaceTables=new Dictionary<string, string>()
            {{"#","Sharp"},{"/","Slash"}}; 

        public ContributeStatisticsByDayStorage(TableStorageConnection connection)
        {
            _connection = connection;
            _contributeStatisticsByDayTable =
                connection.TableClient.GetTableReference(ContributeStatisticsByDayTableName);
            _singleUserLanguageStatisticsByDayTable =
                connection.TableClient.GetTableReference(SingleUserLanguageStatisticsByDayTableName);
            _contributeStatisticsByDayTable.CreateIfNotExists();
            _singleUserLanguageStatisticsByDayTable.CreateIfNotExists();
            _contributeStatisticsByDayDiffTable =
                connection.TableClient.GetTableReference(ContributeStatisticsByDayDiffTableName);
            _singleUserLanguageStatisticsByDayDiffTable =
                connection.TableClient.GetTableReference(SingleUserLanguageStatisticsByDayDiffTableName);
            _contributeStatisticsByDayDiffTable.CreateIfNotExists();
            _singleUserLanguageStatisticsByDayDiffTable.CreateIfNotExists();
        }

        public async Task Add(ContributeStatisticsByDay day)
        {
            foreach (SingleUserLanguageStatisticsByDay statEntity in day.LanguageStatistics)
            {
                foreach (KeyValuePair<string, string> replacePair in ReplaceTables)
                {
                    statEntity.Language = statEntity.Language.Replace(replacePair.Key, replacePair.Value);
                }
                await _singleUserLanguageStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(statEntity));
            }
            await _contributeStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(day));
            await GenerateDiff(day);
        }

        public async Task<ContributeStatisticsByDay> Get(ApplicationUser user, DateTime time)
        {
            return await Get(user.Id.ToString(), time);
        }

        public async Task<ContributeStatisticsByDay> Get(string userId,DateTime time)
        {
            var dateCode = time.ToDateCode().ToString();
            var retrieved =
                await
                    _contributeStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<ContributeStatisticsByDay>(userId, dateCode));
            var result = (ContributeStatisticsByDay)retrieved.Result;
            if (result == null) return null;
            GetRelatedLanguageStatistics(result);
            return result;
        }

        private async Task GenerateDiff(ContributeStatisticsByDay day)
        {
            var yesterdayData = await Get(day.PartitionKey, DateTime.Now - new TimeSpan(1, 0, 0, 0));
            if (yesterdayData == null) return;
            ContributeStatisticsByDayDiff diff = ContributeStatisticsByDayDiff.GenerateTodayDiff(day.PartitionKey, day,
                yesterdayData);
            foreach (SingleUserLanguageStatisticsByDayDiff statEntity in diff.LanguageStatisticsDiffs)
            {
                await _singleUserLanguageStatisticsByDayDiffTable.ExecuteAsync(TableOperation.InsertOrReplace(statEntity));
            }
            await _contributeStatisticsByDayDiffTable.ExecuteAsync(TableOperation.InsertOrReplace(diff));
        }


        /// <summary>
        /// 関連しているLanguageStatisticを取得し格納します。
        /// </summary>
        /// <param name="contribute"></param>
        private void GetRelatedLanguageStatistics(ContributeStatisticsByDay contribute)
        {
            TableQuery<SingleUserLanguageStatisticsByDay> query =new TableQuery<SingleUserLanguageStatisticsByDay>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, contribute.UniqueId));
           var result= _singleUserLanguageStatisticsByDayTable.ExecuteQuery(query);
            contribute.LanguageStatistics=new HashSet<SingleUserLanguageStatisticsByDay>(result);
        }
    }
}