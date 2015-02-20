using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Storage.DataModels.Achivement;

namespace UnitusCore.Storage
{
    public class AchivementStatisticsStorage
    {
        private const string SingleUserAchivementStatisticsByDayName = "SingleUserAchivementStatisticsByDay";

        private const string AchivementBodyName = "AchivementBody";

        private const string AchivementProgressStatisticsByDayName = "AchivementProgressStatisticsByDay";

        private const string AchivementProgressStatisticsForCircleName = "AchivementStatisticsForCircleName";

        private static Dictionary<string, Func<ContributeStatisticsByDay, double>> _achivementsByContributesStatistics = new Dictionary
            <string, Func<ContributeStatisticsByDay, double>>()
        {
            {"ファーストコミット", (s) => s.SumCommit},
            {"累計コミット数:入門", (s) => s.SumCommit/20d},
            {"累計コミット数:初心者", (s) => s.SumCommit/75d},
            {"累計コミット数:初級者", (s) => s.SumCommit/200d},
            {"累計コミット数:中級者", (s) => s.SumCommit/750d},
            {"累計コミット数:上級者", (s) => s.SumCommit/2000d},
            {"累計追加行数:入門", (s) => s.SumAddition/50d},
            {"累計追加行数:初心者", (s) => s.SumAddition/500d},
            {"累計追加行数:初級者", (s) => s.SumAddition/5000d},
            {"累計追加行数:中級者", (s) => s.SumAddition/50000d},
            {"累計追加行数:上級者", (s) => s.SumAddition/500000d},
            {"累計削除行数:入門", (s) => s.SumDeletion/50d},
            {"累計削除行数:初心者", (s) => s.SumDeletion/500d},
            {"累計削除行数:初級者", (s) => s.SumDeletion/5000d},
            {"累計削除行数:中級者", (s) => s.SumDeletion/50000d},
            {"累計削除行数:上級者", (s) => s.SumDeletion/500000d},
            {"累計レポジトリ数:入門", (s) => s.SumRepository},
            {"累計レポジトリ数:初心者", (s) => s.SumRepository/2d},
            {"累計レポジトリ数:初級者", (s) => s.SumRepository/5d},
            {"累計レポジトリ数:中級者", (s) => s.SumRepository/15d},
            {"累計レポジトリ数:上級者", (s) => s.SumRepository/30d},
            {"累計フォーク数:入門", (s) => s.SumForking},
            {"累計フォーク数:初心者", (s) => s.SumForking/2d},
            {"累計フォーク数:初級者", (s) => s.SumForking/5d},
            {"累計フォーク数:中級者", (s) => s.SumForking/8d},
            {"累計フォーク数:上級者", (s) => s.SumForking/15d},
            {"累計被フォーク数:入門", (s) => s.SumForked},
            {"累計被フォーク数:初心者", (s) => s.SumForked/2d},
            {"累計被フォーク数:初級者", (s) => s.SumForked/5d},
            {"累計被フォーク数:中級者", (s) => s.SumForked/8d},
            {"累計被フォーク数:上級者", (s) => s.SumForked/15d}
        };

        private readonly TableStorageConnection _storageConnection;

        private readonly ContributeStatisticsByDayStorage _contributeStorage;

        private readonly CloudTable _singleUserAchivementStatisticsByDayTable;

        private readonly CloudTable _achivementBodyTable;

        private readonly CloudTable _achivementProgressStatisticsByDayTable;

        private readonly CloudTable _achivementStatisticsForCircleTable;

        public IEnumerable<string> GetAchivementNames()
        {
            foreach (var achivement in _achivementsByContributesStatistics)
            {
                yield return achivement.Key;
            }
        }

        public AchivementStatisticsStorage(TableStorageConnection storageConnection)
        {
            _storageConnection = storageConnection;
            _contributeStorage=new ContributeStatisticsByDayStorage(storageConnection);
            _singleUserAchivementStatisticsByDayTable = InitCloudTable(SingleUserAchivementStatisticsByDayName);
            _achivementBodyTable = InitCloudTable(AchivementBodyName);
            _achivementProgressStatisticsByDayTable = InitCloudTable(AchivementProgressStatisticsByDayName);
            _achivementStatisticsForCircleTable = InitCloudTable(AchivementProgressStatisticsForCircleName);
        }

        private CloudTable InitCloudTable(string referenceName)
        {
            var table = _storageConnection.TableClient.GetTableReference(referenceName);
            table.CreateIfNotExists();
            return table;
        }

        /// <summary>
        /// 今日もしくは昨日のデータをとってくる。
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<ContributeStatisticsByDay> GetTodayOrYesterdayStat(ApplicationUser user)
        {
            var today =await _contributeStorage.Get(user, DateTime.Now);
            if (today != null) return today;
            var yesterday = await _contributeStorage.Get(user, DateTime.Now - new TimeSpan(1, 0, 0, 0));
            return yesterday;
        }

        public async Task AddAchivementData(ApplicationUser user,Dictionary<string,Func<ContributeStatisticsByDay,double>> achivements)
        {
            var stat = await GetTodayOrYesterdayStat(user);
            if (stat == null) return;
            foreach (KeyValuePair<string, Func<ContributeStatisticsByDay, double>> achivement in achivements)
            {
                string achivementName = achivement.Key;
                //一つ前の統計データを持ってきて、既に付与されているなら以降考慮しない
                SingleUserAchivementStatisticsByDay achivementData = (SingleUserAchivementStatisticsByDay) (await
                    _singleUserAchivementStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<SingleUserAchivementStatisticsByDay>(achivementName, user.Id))).Result;
                if(achivementData!=null&&achivementData.IsAwarded)continue;
                //実際に実績を評価していく。
                double progress = achivement.Value(stat);
                if (double.IsNaN(progress)) progress = 0;
                progress = Math.Min(Math.Max(progress,0), 1);
                SingleUserAchivementStatisticsByDay newAchivementData=new SingleUserAchivementStatisticsByDay(achivementName,user.Id,progress==1,progress);
                await _singleUserAchivementStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(newAchivementData));
                AchivementProgressStatisticsByDay todayRecord=new AchivementProgressStatisticsByDay(achivementData.UniqueId,progress);
                await _achivementProgressStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(todayRecord));
            }
        }

        public async Task ExecuteAchivementTask(ApplicationUser user)
        {
            await AddAchivementData(user, _achivementsByContributesStatistics);
        }

        public async Task ExecuteAchivementStatisticsBySystemTask(string achivementName)
        {
            AchivementBody body = await RetrieveAchivementBody(achivementName);
            if (body == null) return;
            string query = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, achivementName);
            TableQuery<SingleUserAchivementStatisticsByDay> executableQuery=new TableQuery<SingleUserAchivementStatisticsByDay>().Where(query);
            int count = 0;
            double sumPercentage = 0;
            int sumAwarded = 0;
            foreach (SingleUserAchivementStatisticsByDay entities in _singleUserAchivementStatisticsByDayTable.ExecuteQuery(executableQuery))
            {
                count++;
                if (entities.IsAwarded) sumAwarded++;
                if (!double.IsNaN(sumPercentage)) sumPercentage += entities.CurrentProgress;
            }
            body.AwardedCount = sumAwarded;
            body.AwardedRate = sumAwarded/(double)count;
            body.SumPeople = count;
            body.AvarageProgress = sumPercentage/count;
            await _achivementBodyTable.ExecuteAsync(TableOperation.Replace(body));
        }

        /// <summary>
        /// Achivementを追加したとき用
        /// </summary>
        /// <returns></returns>
        public async Task GenerateToAdjustAchivementBody()
        {
            foreach (string achivementNames in GetAchivementNames())
            {
                var achivementRetrieveResult = await RetrieveAchivementBody(achivementNames);
                if (achivementRetrieveResult == null)
                {
                    AchivementBody achivementBody=new AchivementBody(achivementNames);
                    achivementBody.AchivementDescription = "(詳細未記入)";
                    await _achivementBodyTable.ExecuteAsync(TableOperation.Insert(achivementBody));
                }
            }
        }

        private async Task<AchivementBody> RetrieveAchivementBody(string achivementNames)
        {
            TableResult achivementRetrieveResult =
                await _achivementBodyTable.ExecuteAsync(TableOperation.Retrieve<AchivementBody>("AchivementBody", achivementNames));
            return (AchivementBody) achivementRetrieveResult.Result;
        }

        private async Task<SingleUserAchivementStatisticsByDay> RetrieveAchivementProgressForUser(string achivementName,
            string userId)
        {
            TableResult retrieveResult =
                await
                    _achivementStatisticsForCircleTable.ExecuteAsync(
                        TableOperation.Retrieve<SingleUserAchivementStatisticsByDay>(achivementName, userId));
            return (SingleUserAchivementStatisticsByDay) retrieveResult.Result;
        }

        public async Task UpdateCircleStatistics(ApplicationDbContext dbContext,Circle targetCircle)
        {
            IEnumerable<string> memberUserIds =await targetCircle.GetMemberUserIds(dbContext);
            foreach (string achivementId in GetAchivementNames())
            {
                int memberCount = 0;
                int awardedCount = 0;
                double sumProgress = 0;
                foreach (string userId in memberUserIds)
                {
                    SingleUserAchivementStatisticsByDay data = await RetrieveAchivementProgressForUser(achivementId, userId);
                    if (data != null)
                    {
                        memberCount++;
                        if (data.IsAwarded) awardedCount++;
                        sumProgress += data.CurrentProgress;
                    }
                    AchivementProgressStatisticsForCircle circleData =
                        new AchivementProgressStatisticsForCircle(achivementId, targetCircle.Id.ToString(),
                            sumProgress/memberCount, memberCount, awardedCount, awardedCount/(double) memberCount);
                    await _achivementStatisticsForCircleTable.ExecuteAsync(TableOperation.InsertOrReplace(circleData));
                }
            }
        }
    }
}