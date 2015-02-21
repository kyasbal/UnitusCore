using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Storage.DataModels.Achivement;
using UnitusCore.Util;

namespace UnitusCore.Storage
{
    public class AchivementStatisticsStorage
    {
        private const string SingleUserAchivementStatisticsByDayName = "SingleUserAchivementStatisticsByDay";

        private const string AchivementBodyName = "AchivementBody";

        private const string AchivementProgressStatisticsByDayName = "AchivementProgressStatisticsByDay";

        private const string AchivementProgressStatisticsForCircleName = "AchivementStatisticsForCircle";

        private const string AchivementRankingForCircle = "AchivementRankingForCircle";

        private const string AchivementProgressStatisticsForSystemName = "AchivementStatisticsForSystem";

        private const string AchivementDetailCacheTableName = "AchivementDetailCache";

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

        private readonly CloudTable _achivementStatisticsForSystemTable;

        private readonly CloudTable _achivementRankingForCircleTable;

        private readonly CloudTable _achivementDetailCacheTable;

        public IEnumerable<string> GetAchivementNames()
        {
            foreach (var achivement in _achivementsByContributesStatistics)
            {
                yield return achivement.Key;
            }
        }

        public AchivementStatisticsStorage(TableStorageConnection storageConnection,ApplicationDbContext dbSession)
        {
            _storageConnection = storageConnection;
            _contributeStorage=new ContributeStatisticsByDayStorage(storageConnection,dbSession);
            _singleUserAchivementStatisticsByDayTable = InitCloudTable(SingleUserAchivementStatisticsByDayName);
            _achivementBodyTable = InitCloudTable(AchivementBodyName);
            _achivementProgressStatisticsByDayTable = InitCloudTable(AchivementProgressStatisticsByDayName);
            _achivementStatisticsForCircleTable = InitCloudTable(AchivementProgressStatisticsForCircleName);
            _achivementRankingForCircleTable = InitCloudTable(AchivementRankingForCircle);
            _achivementStatisticsForSystemTable = InitCloudTable(AchivementProgressStatisticsForSystemName);
            _achivementDetailCacheTable = InitCloudTable(AchivementDetailCacheTableName);
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
            return await GetTodayOrYesterdayStat(user.Id);
        }

        private async Task<ContributeStatisticsByDay> GetTodayOrYesterdayStat(string userId)
        {
            var today = await _contributeStorage.Get(userId, DateTime.Now);
            if (today != null) return today;
            var yesterday = await _contributeStorage.Get(userId, DateTime.Now - new TimeSpan(1, 0, 0, 0));
            return yesterday;
        }

        public async Task<string> GetCacheOrCalculate(string achivementName,string userId,Func<Task<string>> calculateAll)
        {
            var cached = await _achivementDetailCacheTable.ExecuteAsync(TableOperation.Retrieve<ResponseCache>("ResponseCache",
                ResponseCache.GetRowKey(achivementName,userId, ResponseCache.ResponseCacheType.Basic)));
            if (cached.Result != null)
            {
                ResponseCache cacheStructed = (ResponseCache) cached.Result;
                return cacheStructed.CachedResponse;
            }
            else
            {
                string newValue =await calculateAll();
                ResponseCache cache=new ResponseCache(achivementName,userId,newValue);
                await _achivementDetailCacheTable.ExecuteAsync(TableOperation.InsertOrReplace(cache));
                return newValue;
            }
        }

        public async Task RemoveAllCachedResult()
        {
            var queryString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "ResponseCache");
            TableQuery<ResponseCache> allQuery = new TableQuery<ResponseCache>().Where(queryString);
            var result=_achivementDetailCacheTable.ExecuteQuery(allQuery);
            foreach (ResponseCache cache in result)
            {
                await _achivementDetailCacheTable.ExecuteAsync(TableOperation.Delete(cache));
            }
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
                double progressDiff = achivementData != null ? progress - achivementData.CurrentProgress : double.NaN;
                SingleUserAchivementStatisticsByDay newAchivementData=new SingleUserAchivementStatisticsByDay(achivementName,user.Id,progress==1,progress,progressDiff);
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
            await
                _achivementStatisticsForSystemTable.ExecuteAsync(
                    TableOperation.InsertOrReplace(new AchivementProgressStatisticsForSystem(achivementName, count,
                        sumAwarded,sumPercentage)));
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

        public async Task<AchivementBody> RetrieveAchivementBody(string achivementNames)
        {
            TableResult achivementRetrieveResult =
                await _achivementBodyTable.ExecuteAsync(TableOperation.Retrieve<AchivementBody>("AchivementBody", achivementNames));
            return (AchivementBody) achivementRetrieveResult.Result;
        }

        public async Task<SingleUserAchivementStatisticsByDay> RetrieveAchivementProgressForUser(string achivementName,
            string userId)
        {
            TableResult retrieveResult =
                await
                    _singleUserAchivementStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<SingleUserAchivementStatisticsByDay>(achivementName, userId));
            return (SingleUserAchivementStatisticsByDay) retrieveResult.Result;
        }

        private async Task<AchivementProgressStatisticsForSystem> RetrieveAchivementProgressForSystem(string achivementName,
    DateTime time)
        {
            TableResult retrieveResult =
                await
                    _achivementStatisticsForSystemTable.ExecuteAsync(
                        TableOperation.Retrieve<AchivementProgressStatisticsForSystem>(achivementName,
                            time.ToDateCode().ToString()));
            return (AchivementProgressStatisticsForSystem)retrieveResult.Result;
        }


        public async Task UpdateCircleStatistics(ApplicationDbContext dbContext,Circle targetCircle)
        {
            IEnumerable<string> memberUserIds =await targetCircle.GetMemberUserIds(dbContext);
            var comparator = new MemberAchivementRankingComparator();
            foreach (string achivementId in GetAchivementNames())
            {
                int memberCount = 0;
                int awardedCount = 0;
                double sumProgress = 0;
                List<SingleUserAchivementStatisticsByDay> userAchivementStatistics=new List<SingleUserAchivementStatisticsByDay>();
                foreach (string userId in memberUserIds)
                {
                    SingleUserAchivementStatisticsByDay data = await RetrieveAchivementProgressForUser(achivementId, userId);
                    if (data != null)
                    {
                        memberCount++;
                        if (data.IsAwarded) awardedCount++;
                        sumProgress += data.CurrentProgress;
                        userAchivementStatistics.Add(data);
                    }
                    AchivementProgressStatisticsForCircle circleData =
                        new AchivementProgressStatisticsForCircle(achivementId, targetCircle.Id.ToString(),
                            sumProgress/memberCount, memberCount, awardedCount, awardedCount/(double) memberCount);
                    await _achivementStatisticsForCircleTable.ExecuteAsync(TableOperation.InsertOrReplace(circleData));
                }
                    userAchivementStatistics.Sort(comparator);
                int rank = 1;
                foreach (var achivementStat in userAchivementStatistics)
                {
                    string userId = achivementStat.UserId;
                    AchivementCircleRankingStatistics rankStat =
                        new AchivementCircleRankingStatistics(targetCircle.Id.ToString(), userId, achivementId, rank);
                    await _achivementRankingForCircleTable.ExecuteAsync(TableOperation.InsertOrReplace(rankStat));
                    rank++;
                }
            }
        }

        public async Task<T> RetrieveUserAchivement<T>(string userId,string achivementName,Func<AchivementBody,SingleUserAchivementStatisticsByDay,ProgressGraphPointGenerator,T> f)
        {
            AchivementBody b = await RetrieveAchivementBody(achivementName);
            SingleUserAchivementStatisticsByDay a = await RetrieveAchivementProgressForUser(achivementName, userId);
            return f(b,a,new ProgressGraphPointGenerator(this,a));
        }

        public async Task<AchivementProgressStatisticsByDay> RetrieveAchivementProgress(SingleUserAchivementStatisticsByDay parent,DateTime time)
        {
            return
                (AchivementProgressStatisticsByDay) (await
                    _achivementProgressStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<AchivementProgressStatisticsByDay>(parent.UniqueId,
                            time.ToDateCode().ToString()))).Result;
        }

        /// <summary>
        /// ユーザーの持っている実績全統計データに関する処理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userIds"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> EachForUserAchivements<T>(string userIds,Func<SingleUserAchivementStatisticsByDay,T> f)
        {
            HashSet<T> result = new HashSet<T>();
            foreach (string achivement in GetAchivementNames())
            {
                var achivementData=await RetrieveAchivementProgressForUser(achivement, userIds);
                result.Add(f(achivementData));
            }
            return result;
        }

        private class MemberAchivementRankingComparator:IComparer<SingleUserAchivementStatisticsByDay>
        {
            public int Compare(SingleUserAchivementStatisticsByDay x, SingleUserAchivementStatisticsByDay y)
            {
                if (x.IsAwarded && !y.IsAwarded)
                {
                    return -1;
                }else if (!x.IsAwarded && y.IsAwarded)
                {
                    return 1;
                }else if (x.IsAwarded && y.IsAwarded)
                {
                    return (int) (x.AwardedDate - y.AwardedDate);
                }
                else
                {
                    return (int) ((y.CurrentProgress-x.CurrentProgress)*100000d);
                }
            }
        }

        public class ProgressGraphPointGenerator
        {
            private readonly AchivementStatisticsStorage _storage;
            private readonly SingleUserAchivementStatisticsByDay _achivementData;

            public ProgressGraphPointGenerator(AchivementStatisticsStorage storage,SingleUserAchivementStatisticsByDay achivementData)
            {
                _storage = storage;
                _achivementData = achivementData;
            }
            /// <summary>
            /// 進捗率のグラフ用の配列を作成
            /// </summary>
            /// <param name="count"></param>
            /// <param name="duration"></param>
            /// <returns></returns>
            public async Task<string[][]> GenerateFromLastForUser(int count, TimeSpan duration)
            {
                var lastStat=await _storage.GetTodayOrYesterdayStat(_achivementData.UserId);
                DateTime beginTime = lastStat == null ? DateTime.Today : lastStat.Timestamp.Date;
                Func<DateTime, Task<double>> progressInjection = FromDateTimeForUserAchivementProgress;
                if (lastStat == null) progressInjection = AlwaysZero;
                List<string> dateLabels=new List<string>();
                List<string> data=new List<string>();
                for (int i = 0; i < count; i++)
                {
                    DateTime sampleTime = beginTime - MultiplyTimeSpan(duration, i);
                    dateLabels.Add(sampleTime.ToString("M月d日"));
                    data.Add((await progressInjection(sampleTime)).ToString());
                }
                dateLabels.Add("日付");
                data.Add("進捗率");
                dateLabels.Reverse();
                data.Reverse();
                return new string[][]
                {
                    dateLabels.ToArray(), data.ToArray()
                };
            }
            /// <summary>
            /// 収得率のグラフ用のデータ配列を作成
            /// </summary>
            /// <param name="count"></param>
            /// <param name="duration"></param>
            /// <returns></returns>
            public async Task<string[][]> GenerateFromLastForSystem(int count,TimeSpan duration)
            {
                var lastStat = await _storage.GetTodayOrYesterdayStat(_achivementData.UserId);
                DateTime beginTime = lastStat == null ? DateTime.Today : lastStat.Timestamp.Date;
                Func<DateTime, Task<Tuple<double,double>>> progressInjection = FromDateTimeForSystemAchivementProgress;
                if (lastStat == null) progressInjection = AlwaysZeroTuple;
                List<string> dateLabels = new List<string>();
                List<string> data = new List<string>();
                List<string> sumData=new List<string>();
                for (int i = 0; i < count; i++)
                {
                    DateTime sampleTime = beginTime - MultiplyTimeSpan(duration, i);
                    dateLabels.Add(sampleTime.ToString("M月d日"));
                    var injected = (await progressInjection(sampleTime));
                    data.Add(injected.Item1.ToString());
                    sumData.Add(injected.Item2.ToString());
                }
                dateLabels.Add("日付");
                data.Add("進捗率");
                sumData.Add("全体人数");
                dateLabels.Reverse();
                data.Reverse();
                sumData.Reverse();
                return new string[][]
                {
                    dateLabels.ToArray(), data.ToArray(),sumData.ToArray()
                };
            }

            private TimeSpan MultiplyTimeSpan(TimeSpan span, int c)
            {
                TimeSpan spanResult=new TimeSpan();
                for (int i = 0; i < c; i++)
                {
                    spanResult += span;
                }
                return spanResult;
            }

            private async Task<double> FromDateTimeForUserAchivementProgress(DateTime t)
            {
                var data = await _storage.RetrieveAchivementProgress(_achivementData, t);
                return data == null ? 0 : data.Progress;
            }

            private async Task<Tuple<double,double>> FromDateTimeForSystemAchivementProgress(DateTime t)
            {
                var data = await _storage.RetrieveAchivementProgressForSystem(_achivementData.AchivementId,t);
                return data == null
                    ? new Tuple<double, double>(0, 0)
                    : new Tuple<double, double>(data.AwardedPeople, data.SumPeople);
            }

            private async Task<double> AlwaysZero(DateTime t)
            {
                return 0d;
            }

            private async Task<Tuple<double,double>> AlwaysZeroTuple(DateTime t)
            {
                return new Tuple<double, double>(0, 0);
            }
        }

        public async Task<IOrderedEnumerable<AchivementCircleRankingStatistics>> GetRankingList(string circleId, string achivementName)
        {
            string partitionKey = achivementName + "-" + circleId;
            string cond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            TableQuery<AchivementCircleRankingStatistics> rankingQuery=new TableQuery<AchivementCircleRankingStatistics>().Where(cond);
            return _achivementRankingForCircleTable.ExecuteQuery(rankingQuery).OrderBy(a=>a.Rank);
        }
    }
}