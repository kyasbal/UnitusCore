using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Storage.DataModels.Achivement;
using UnitusCore.Util;

namespace UnitusCore.Storage
{
    public class AchivementStatisticsStorage:TableStorageBase
    {
        private const string AchivementCategoryTableName = "AchivementCategories";

        private const string SingleUserAchivementStatisticsByDayTableName = "SingleUserAchivementStatisticsByDay";

        private const string AchivementBodyTableName = "AchivementBody";

        private const string AchivementProgressStatisticsByDayTableName = "AchivementProgressStatisticsByDay";

        private const string AchivementProgressStatisticsForCircleTableName = "AchivementStatisticsForCircle";

        private const string AchivementRankingForCircleTableName = "AchivementRankingForCircle";

        private const string AchivementProgressStatisticsForSystemTableName = "AchivementStatisticsForSystem";

        private const string AchivementDetailCacheTableName = "AchivementDetailCache";

        private const string AchivementProgressLogForCircleTableName = "AchivementProgressLogForCircle";

        private static readonly Dictionary<string, Func<ContributeStatisticsByDay, double>>
            _achivementsByContributesStatistics = new Dictionary
                <string, Func<ContributeStatisticsByDay, double>>
            {
                {"ファーストコミット", s => s.SumCommit},
                {"累計コミット数:入門", s => s.SumCommit/20d},
                {"累計コミット数:初心者", s => s.SumCommit/75d},
                {"累計コミット数:初級者", s => s.SumCommit/200d},
                {"累計コミット数:中級者", s => s.SumCommit/750d},
                {"累計コミット数:上級者", s => s.SumCommit/2000d},
                {"累計追加行数:入門", s => s.SumAddition/50d},
                {"累計追加行数:初心者", s => s.SumAddition/500d},
                {"累計追加行数:初級者", s => s.SumAddition/5000d},
                {"累計追加行数:中級者", s => s.SumAddition/50000d},
                {"累計追加行数:上級者", s => s.SumAddition/500000d},
                {"累計削除行数:入門", s => s.SumDeletion/50d},
                {"累計削除行数:初心者", s => s.SumDeletion/500d},
                {"累計削除行数:初級者", s => s.SumDeletion/5000d},
                {"累計削除行数:中級者", s => s.SumDeletion/50000d},
                {"累計削除行数:上級者", s => s.SumDeletion/500000d},
                {"累計レポジトリ数:入門", s => s.SumRepository},
                {"累計レポジトリ数:初心者", s => s.SumRepository/2d},
                {"累計レポジトリ数:初級者", s => s.SumRepository/5d},
                {"累計レポジトリ数:中級者", s => s.SumRepository/15d},
                {"累計レポジトリ数:上級者", s => s.SumRepository/30d},
                {"累計フォーク数:入門", s => s.SumForking},
                {"累計フォーク数:初心者", s => s.SumForking/2d},
                {"累計フォーク数:初級者", s => s.SumForking/5d},
                {"累計フォーク数:中級者", s => s.SumForking/8d},
                {"累計フォーク数:上級者", s => s.SumForking/15d},
                {"累計被フォーク数:入門", s => s.SumForked},
                {"累計被フォーク数:初心者", s => s.SumForked/2d},
                {"累計被フォーク数:初級者", s => s.SumForked/5d},
                {"累計被フォーク数:中級者", s => s.SumForked/8d},
                {"累計被フォーク数:上級者", s => s.SumForked/15d}
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

        private readonly CloudTable _achivementProgressLogForCircleTable;

        private readonly CloudTable _achivementCategoriesTable;

        public IEnumerable<string> GetAchivementNames()
        {
            foreach (var achivement in _achivementsByContributesStatistics)
            {
                yield return achivement.Key;
            }
        }

        public AchivementStatisticsStorage(TableStorageConnection storageConnection, ApplicationDbContext dbSession):base(storageConnection)
        {
            _storageConnection = storageConnection;
            _contributeStorage = new ContributeStatisticsByDayStorage(storageConnection, dbSession);
            _singleUserAchivementStatisticsByDayTable = InitCloudTable(SingleUserAchivementStatisticsByDayTableName);
            _achivementBodyTable = InitCloudTable(AchivementBodyTableName);
            _achivementProgressStatisticsByDayTable = InitCloudTable(AchivementProgressStatisticsByDayTableName);
            _achivementStatisticsForCircleTable = InitCloudTable(AchivementProgressStatisticsForCircleTableName);
            _achivementRankingForCircleTable = InitCloudTable(AchivementRankingForCircleTableName);
            _achivementStatisticsForSystemTable = InitCloudTable(AchivementProgressStatisticsForSystemTableName);
            _achivementDetailCacheTable = InitCloudTable(AchivementDetailCacheTableName);
            _achivementProgressLogForCircleTable = InitCloudTable(AchivementProgressLogForCircleTableName);
            _achivementCategoriesTable = InitCloudTable(AchivementCategoryTableName);
        }

        /// <summary>
        ///     今日もしくは昨日のデータをとってくる。
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

        public async Task<string> GetCacheOrCalculate(string achivementName, string userId,
            Func<Task<string>> calculateAll)
        {
            var cached =
                await _achivementDetailCacheTable.ExecuteAsync(TableOperation.Retrieve<ResponseCache>("ResponseCache",
                    ResponseCache.GetRowKey(achivementName, userId, ResponseCache.ResponseCacheType.Basic)));
            if (cached.Result != null)
            {
                var cacheStructed = (ResponseCache) cached.Result;
                return cacheStructed.CachedResponse;
            }
            var newValue = await calculateAll();
            var cache = new ResponseCache(achivementName, userId, newValue);
            await _achivementDetailCacheTable.ExecuteAsync(TableOperation.InsertOrReplace(cache));
            return newValue;
        }

        public async Task RemoveAllCachedResult()
        {
            var queryString = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "ResponseCache");
            var allQuery = new TableQuery<ResponseCache>().Where(queryString);
            var result = _achivementDetailCacheTable.ExecuteQuery(allQuery);
            foreach (var cache in result)
            {
                await _achivementDetailCacheTable.ExecuteAsync(TableOperation.Delete(cache));
            }
        }

        public async Task AddAchivementData(ApplicationUser user,
            Dictionary<string, Func<ContributeStatisticsByDay, double>> achivements)
        {
            var stat = await GetTodayOrYesterdayStat(user);
            if (stat == null) return;
            foreach (var achivement in achivements)
            {
                var achivementName = achivement.Key;
                //一つ前の統計データを持ってきて、既に付与されているなら以降考慮しない
                var achivementData = (SingleUserAchivementStatisticsByDay) (await
                    _singleUserAchivementStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<SingleUserAchivementStatisticsByDay>(achivementName, user.Id))).Result;
                if (achivementData != null && achivementData.IsAwarded) continue;
                //実際に実績を評価していく。
                var progress = achivement.Value(stat);
                if (double.IsNaN(progress)) progress = 0;
                progress = Math.Min(Math.Max(progress, 0), 1);
                var progressDiff = achivementData != null ? progress - achivementData.CurrentProgress : double.NaN;
                var newAchivementData = new SingleUserAchivementStatisticsByDay(achivementName, user.Id, progress == 1,
                    progress, progressDiff);
                await
                    _singleUserAchivementStatisticsByDayTable.ExecuteAsync(
                        TableOperation.InsertOrReplace(newAchivementData));
                var todayRecord = new AchivementProgressStatisticsByDay(newAchivementData.UniqueId, progress);
                await _achivementProgressStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(todayRecord));
            }
        }

        public async Task ExecuteAchivementTask(ApplicationUser user)
        {
            await AddAchivementData(user, _achivementsByContributesStatistics);
        }

        public async Task ExecuteAchivementStatisticsBySystemTask(string achivementName)
        {
            var body = await RetrieveAchivementBody(achivementName);
            if (body == null) return;
            var query = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, achivementName);
            var executableQuery = new TableQuery<SingleUserAchivementStatisticsByDay>().Where(query);
            var count = 0;
            double sumPercentage = 0;
            var sumAwarded = 0;
            foreach (var entities in _singleUserAchivementStatisticsByDayTable.ExecuteQuery(executableQuery))
            {
                count++;
                if (entities.IsAwarded) sumAwarded++;
                if (!double.IsNaN(sumPercentage)) sumPercentage += entities.CurrentProgress;
            }
            await
                _achivementStatisticsForSystemTable.ExecuteAsync(
                    TableOperation.InsertOrReplace(new AchivementProgressStatisticsForSystem(achivementName, count,
                        sumAwarded, sumPercentage)));
            body.AwardedCount = sumAwarded;
            body.AwardedRate = sumAwarded/(double) count;
            body.SumPeople = count;
            body.AvarageProgress = sumPercentage/count;
            await _achivementBodyTable.ExecuteAsync(TableOperation.Replace(body));
        }

        /// <summary>
        ///     Achivementを追加したとき用
        /// </summary>
        /// <returns></returns>
        public async Task GenerateToAdjustAchivementBody()
        {
            foreach (var achivementNames in GetAchivementNames())
            {
                var achivementRetrieveResult = await RetrieveAchivementBody(achivementNames);
                if (achivementRetrieveResult == null)
                {
                    var achivementBody = new AchivementBody(achivementNames);
                    achivementBody.AchivementDescription = "(詳細未記入)";
                    await _achivementBodyTable.ExecuteAsync(TableOperation.Insert(achivementBody));
                }
            }
        }

        public async Task<AchivementBody> RetrieveAchivementBody(string achivementNames)
        {
            var achivementRetrieveResult =
                await
                    _achivementBodyTable.ExecuteAsync(TableOperation.Retrieve<AchivementBody>("AchivementBody",
                        achivementNames));
            return (AchivementBody) achivementRetrieveResult.Result;
        }

        public async Task<SingleUserAchivementStatisticsByDay> RetrieveAchivementProgressForUser(string achivementName,
            string userId)
        {
            var retrieveResult =
                await
                    _singleUserAchivementStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<SingleUserAchivementStatisticsByDay>(achivementName, userId));
            return (SingleUserAchivementStatisticsByDay) retrieveResult.Result;
        }

        private async Task<AchivementProgressStatisticsForSystem> RetrieveAchivementProgressForSystem(
            string achivementName,
            DateTime time)
        {
            var retrieveResult =
                await
                    _achivementStatisticsForSystemTable.ExecuteAsync(
                        TableOperation.Retrieve<AchivementProgressStatisticsForSystem>(achivementName,
                            time.ToDateCode().ToString()));
            return (AchivementProgressStatisticsForSystem) retrieveResult.Result;
        }

        private async Task<AchivementProgressStatisticsForCircleLog> RetrieveProgressForCircle(string achivementName,
            string circleId,
            DateTime time)
        {
            var retrieveResult =
                await
                    _achivementProgressLogForCircleTable.ExecuteAsync(
                        TableOperation.Retrieve<AchivementProgressStatisticsForCircleLog>(
                            achivementName + "-" + circleId, time.ToUnixTime().ToString()));
            return (AchivementProgressStatisticsForCircleLog) retrieveResult.Result;
        }


        public async Task UpdateCircleStatistics(ApplicationDbContext dbContext, Circle targetCircle)
        {
            var memberUserIds = await targetCircle.GetMemberUserIds(dbContext);
            var comparator = new MemberAchivementRankingComparator();
            foreach (var achivementId in GetAchivementNames())
            {
                var memberCount = 0;
                var awardedCount = 0;
                double sumProgress = 0;
                var userAchivementStatistics = new List<SingleUserAchivementStatisticsByDay>();
                foreach (var userId in memberUserIds) //ユーザー名とってくる
                {
                    var data =
                        await RetrieveAchivementProgressForUser(achivementId, userId); //ユーザーの前回のログ
                    if (data != null)
                    {
                        memberCount++;
                        if (data.IsAwarded) awardedCount++;
                        sumProgress += data.CurrentProgress;
                        userAchivementStatistics.Add(data);
                    }
                }
                //データの作成&挿入
                var circleData =
                    new AchivementProgressStatisticsForCircle(achivementId, targetCircle.Id.ToString(),
                        sumProgress/memberCount, memberCount, awardedCount, awardedCount/(double) memberCount);
                await _achivementStatisticsForCircleTable.ExecuteAsync(TableOperation.InsertOrReplace(circleData));

                //時間ベースのほうのデータも作成
                var circleDataLog = new AchivementProgressStatisticsForCircleLog(circleData);
                await _achivementProgressLogForCircleTable.ExecuteAsync(TableOperation.InsertOrReplace(circleDataLog));

                userAchivementStatistics.Sort(comparator);
                var rank = 1;
                foreach (var achivementStat in userAchivementStatistics)
                {
                    var userId = achivementStat.UserId;
                    var rankStat =
                        new AchivementCircleRankingStatistics(targetCircle.Id.ToString(), userId, achivementId, rank);
                    await _achivementRankingForCircleTable.ExecuteAsync(TableOperation.InsertOrReplace(rankStat));
                    rank++;
                }
            }
        }

        public async Task<T> RetrieveUserAchivement<T>(string userId, string achivementName,
            Func<AchivementBody, SingleUserAchivementStatisticsByDay, ProgressGraphPointGenerator, T> f,
            ApplicationDbContext dbContext, ApplicationUser user)
        {
            var b = await RetrieveAchivementBody(achivementName);
            var a = await RetrieveAchivementProgressForUser(achivementName, userId);
            return f(b, a, new ProgressGraphPointGenerator(user, dbContext, this, a));
        }

        public async Task<AchivementProgressStatisticsByDay> RetrieveAchivementProgress(
            SingleUserAchivementStatisticsByDay parent, DateTime time)
        {
            return
                (AchivementProgressStatisticsByDay) (await
                    _achivementProgressStatisticsByDayTable.ExecuteAsync(
                        TableOperation.Retrieve<AchivementProgressStatisticsByDay>(parent.UniqueId,
                            time.ToDateCode().ToString()))).Result;
        }

        private IEnumerable<string> RetrieveAchivementNamesForCategory(string categoryName)
        {
            TableQuery<CategoryAchivementPair> pairRetrieveQuery=new TableQuery<CategoryAchivementPair>().Where(TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,categoryName));
            return _achivementCategoriesTable.ExecuteQuery(pairRetrieveQuery).Select(a => a.AchivementName);
        }

        /// <summary>
        ///     ユーザーの持っている実績全統計データに関する処理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userIds"></param>
        /// <param name="f"></param>
        /// <param name="achivementCategory"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> EachForUserAchivements<T>(string userIds, Func<SingleUserAchivementStatisticsByDay, AchivementStatisticsStorage, T> f, string achivementCategory)
        {
            var result = new HashSet<T>();
            IEnumerable<string> targetAchivementNames = null;
            if (achivementCategory.Equals("全て"))
            {
                targetAchivementNames = GetAchivementNames();
            }
            else
            {
                targetAchivementNames = RetrieveAchivementNamesForCategory(achivementCategory);
            }
            foreach (var achivement in targetAchivementNames)
            {
                var achivementData = await RetrieveAchivementProgressForUser(achivement, userIds);
                if (achivementData == null)
                {
                    achivementData = new SingleUserAchivementStatisticsByDay(achivement, userIds, false, double.NaN,
                        double.NaN);
                }
                result.Add(f(achivementData, this));
            }
            return result;
        }

        public async Task<IOrderedEnumerable<AchivementCircleRankingStatistics>> GetRankingList(string circleId,
            string achivementName)
        {
            var partitionKey = achivementName + "-" + circleId;
            var cond = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            var rankingQuery = new TableQuery<AchivementCircleRankingStatistics>().Where(cond);
            return _achivementRankingForCircleTable.ExecuteQuery(rankingQuery).OrderBy(a => a.Rank);
        }

        public async Task<IEnumerable<string>> GetAchivementCategories()
        {
            TableQuery<CategoryNamesEntity> categoryQuery=new TableQuery<CategoryNamesEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,"CATEGORY"));
            return _achivementCategoriesTable.ExecuteQuery(categoryQuery).Select(a=>a.CategoryName);
        }

        public async Task Maintenance_GenerateAchivementCategoriesForExisitingAchivementBody()
        {
            foreach (string achivementName in GetAchivementNames())
            {
                TableQuery<CategoryAchivementPair> achivementPairs=new TableQuery<CategoryAchivementPair>().Where(TableQuery.GenerateFilterCondition("RowKey",QueryComparisons.Equal,achivementName));
                var queryResult = _achivementCategoriesTable.ExecuteQuery(achivementPairs);
                if (queryResult.Count() == 0)
                {
                    await _achivementCategoriesTable.ExecuteAsync(
                        TableOperation.Insert(new CategoryAchivementPair(achivementName, "github")));
                }
            }
        }

        private class MemberAchivementRankingComparator : IComparer<SingleUserAchivementStatisticsByDay>
        {
            public int Compare(SingleUserAchivementStatisticsByDay x, SingleUserAchivementStatisticsByDay y)
            {
                if (x.IsAwarded && !y.IsAwarded)
                {
                    return -1;
                }
                if (!x.IsAwarded && y.IsAwarded)
                {
                    return 1;
                }
                if (x.IsAwarded && y.IsAwarded)
                {
                    return (int) (x.AwardedDate - y.AwardedDate);
                }
                return (int) ((y.CurrentProgress - x.CurrentProgress)*100000d);
            }
        }

        public class ProgressGraphPointGenerator
        {
            private class NameDataPair
            {
                public NameDataPair(double[] data, string name)
                {
                    this.data = data;
                    this.name = name;
                }

                public double[] data;

                public string name;
            }

            private const string UserProgressHistoryLabelName = "進捗率";
            private const string SystemProgressAvarageHistoryLabelName = "UNITUS全体の平均";

            private readonly ApplicationUser _user;
            private readonly ApplicationDbContext _dbContext;
            private readonly AchivementStatisticsStorage _storage;
            private readonly SingleUserAchivementStatisticsByDay _achivementData;

            public ProgressGraphPointGenerator(ApplicationUser user, ApplicationDbContext dbContext,
                AchivementStatisticsStorage storage, SingleUserAchivementStatisticsByDay achivementData)
            {
                _user = user;
                _dbContext = dbContext;
                _storage = storage;
                _achivementData = achivementData;
            }

            private async Task<NameDataPair> GetUserProgressHistory(DateTime beginTime, int count, TimeSpan duration)
            {
                var progressHistory = new List<double>();
                for (var i = 0; i < count; i++)
                {
                    var sampleTime = beginTime - MultiplyTimeSpan(duration, i); //サンプルする時刻を取得
                    var sampledData = await _storage.RetrieveAchivementProgress(_achivementData, sampleTime);
                    if (sampledData == null) progressHistory.Add(0);
                    else
                    {
                        progressHistory.Add(sampledData.Progress);
                    }
                }
                progressHistory.Reverse();
                return new NameDataPair(progressHistory.ToArray(), UserProgressHistoryLabelName);
            }

            private async Task<NameDataPair> GetSystemProgressAverageHistory(DateTime beginTime, int count,
                TimeSpan duration)
            {
                var progressHistory = new List<double>();
                for (var i = 0; i < count; i++)
                {
                    var sampleTime = beginTime - MultiplyTimeSpan(duration, i);
                    var sampledData =
                        await _storage.RetrieveAchivementProgressForSystem(_achivementData.AchivementId, sampleTime);
                    progressHistory.Add(sampledData == null ? 0 : sampledData.AveragePercentage);
                }
                progressHistory.Reverse();
                return new NameDataPair(progressHistory.ToArray(), SystemProgressAvarageHistoryLabelName);
            }

            private async Task<NameDataPair> GetCircleProgressAverageHistory(DateTime beginTime, int count,
                TimeSpan duration, Circle circle)
            {
                var progressHistory = new List<double>();
                for (var i = 0; i < count; i++)
                {
                    var sampleTime = beginTime - MultiplyTimeSpan(duration, i);
                    var sampledData =
                        await
                            _storage.RetrieveProgressForCircle(_achivementData.AchivementId, circle.Id.ToString(),
                                sampleTime);
                    progressHistory.Add(sampledData == null ? 0 : sampledData.AvrProgress);
                }
                progressHistory.Reverse();
                return new NameDataPair(progressHistory.ToArray(), string.Format("「{0}」内の平均進捗率", circle.Name));
            }

            /// <summary>
            ///     進捗率のグラフ用の配列を作成
            /// </summary>
            /// <param name="count"></param>
            /// <param name="duration"></param>
            /// <returns></returns>
            public async Task<dynamic> GenerateFromLastForUser(int count, TimeSpan duration)
            {
                //統計をとる最初の日を決める
                var lastStat = await _storage.GetTodayOrYesterdayStat(_achivementData.UserId);
                var beginTime = lastStat == null ? DateTime.Today : lastStat.Timestamp.Date;
                //X軸のラベルを決定
                var dateLabels = new List<string>();
                for (var i = 0; i < count; i++)
                {
                    var sampleTime = beginTime - MultiplyTimeSpan(duration, i);
                    dateLabels.Add(sampleTime.ToString("M月d日"));
                }
                dateLabels.Reverse();
                //グラフのデータを生成する
                var graphPoints = new List<NameDataPair>();
                graphPoints.Add(await GetUserProgressHistory(beginTime, count, duration));
                graphPoints.Add(await GetSystemProgressAverageHistory(beginTime, count, duration));
                //サークルごとのグラフデータ生成
                var circles = await _user.RetrieveBelongingCircles(_dbContext);
                foreach (var circle in circles)
                {
                    graphPoints.Add(await GetCircleProgressAverageHistory(beginTime, count, duration, circle));
                }
                return new
                {
                    xAxis = new
                    {
                        categories = dateLabels.ToArray()
                    },
                    series = graphPoints.ToArray()
                };
            }

            /// <summary>
            ///     収得率のグラフ用のデータ配列を作成
            /// </summary>
            /// <param name="count"></param>
            /// <param name="duration"></param>
            /// <returns></returns>
            public async Task<string[][]> GenerateFromLastForSystem(int count, TimeSpan duration)
            {
                var lastStat = await _storage.GetTodayOrYesterdayStat(_achivementData.UserId);
                var beginTime = lastStat == null ? DateTime.Today : lastStat.Timestamp.Date;
                Func<DateTime, Task<Tuple<double, double>>> progressInjection = FromDateTimeForSystemAchivementProgress;
                if (lastStat == null) progressInjection = AlwaysZeroTuple;
                var dateLabels = new List<string>();
                var data = new List<string>();
                var sumData = new List<string>();
                for (var i = 0; i < count; i++)
                {
                    var sampleTime = beginTime - MultiplyTimeSpan(duration, i);
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
                return new[]
                {
                    dateLabels.ToArray(), data.ToArray(), sumData.ToArray()
                };
            }

            private TimeSpan MultiplyTimeSpan(TimeSpan span, int c)
            {
                var spanResult = new TimeSpan();
                for (var i = 0; i < c; i++)
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

            private async Task<Tuple<double, double>> FromDateTimeForSystemAchivementProgress(DateTime t)
            {
                var data = await _storage.RetrieveAchivementProgressForSystem(_achivementData.AchivementId, t);
                return data == null
                    ? new Tuple<double, double>(0, 0)
                    : new Tuple<double, double>(data.AwardedPeople, data.SumPeople);
            }

            private async Task<double> AlwaysZero(DateTime t)
            {
                return 0d;
            }

            private async Task<Tuple<double, double>> AlwaysZeroTuple(DateTime t)
            {
                return new Tuple<double, double>(0, 0);
            }
        }
    }
}