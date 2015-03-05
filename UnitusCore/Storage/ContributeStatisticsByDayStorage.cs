﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using Octokit;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Util;
using UnitusCore.Util.Github;

namespace UnitusCore.Storage
{
    public class ContributeStatisticsByDayStorage:TableStorageBase
    {
        private const string ContributeStatisticsByDayTableName = "StatisticsContributeByDay";

        private const string SingleUserLanguageStatisticsByDayTableName = "StatisticsSingleUserLanguageByDay";

        private const string ContributeStatisticsByDayDiffTableName = "StatisticsContributeByDayDiff";

        private const string SingleUserLanguageStatisticsByDayDiffTableName = "StatisticsSingleUserLanguageByDayDiff";

        private const string CollabolatorCountByDayTableName = "CollabolatorCountByDayTable";

        private const string GistContributeStatisticsForSingleUserTableName = "GistContributionsForSingleUser";

        private const string GistContributeStatisticsByLanguageForSingleUserTablename = "GistContributionsByLanguageForSingleUser";

        private readonly TableStorageConnection _connection;
        private readonly ApplicationDbContext _dbContext;

        private readonly CloudTable _contributeStatisticsByDayTable;

        private readonly CloudTable _singleUserLanguageStatisticsByDayTable;

        private readonly CloudTable _contributeStatisticsByDayDiffTable;

        private readonly CloudTable _singleUserLanguageStatisticsByDayDiffTable;

        private readonly CloudTable _collaboratorCountByDayTable;

        private readonly CloudTable _gistContributeStatisticsForSingleUser;

        private readonly CloudTable _gistContributeStatisticsByLanguageForSingleUser;

        public ContributeStatisticsByDayStorage(TableStorageConnection connection, ApplicationDbContext dbContext):base(connection)
        {
            _connection = connection;
            _dbContext = dbContext;
            _contributeStatisticsByDayTable =InitCloudTable(ContributeStatisticsByDayTableName);
            _singleUserLanguageStatisticsByDayTable =InitCloudTable(SingleUserLanguageStatisticsByDayTableName);
            _contributeStatisticsByDayDiffTable =InitCloudTable(ContributeStatisticsByDayDiffTableName);
            _singleUserLanguageStatisticsByDayDiffTable =InitCloudTable(SingleUserLanguageStatisticsByDayDiffTableName);
            _collaboratorCountByDayTable = InitCloudTable(CollabolatorCountByDayTableName);
            _gistContributeStatisticsForSingleUser = InitCloudTable(GistContributeStatisticsForSingleUserTableName);
            _gistContributeStatisticsByLanguageForSingleUser =
                InitCloudTable(GistContributeStatisticsByLanguageForSingleUserTablename);
        }

        public async Task Add(ContributeStatisticsByDay day, ContributionAnalysis.CollaboratorDictionary collaboratorLog)
        {
            //Collaborator関連
            await StatCollaborators(day, collaboratorLog);
            foreach (SingleUserLanguageStatisticsByDay statEntity in day.LanguageStatistics)
            {
                statEntity.Language=statEntity.Language.ToSafeForTable();
                await _singleUserLanguageStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(statEntity));
            }
            await _contributeStatisticsByDayTable.ExecuteAsync(TableOperation.InsertOrReplace(day));
            await GenerateDiff(day);
        }

        private async Task StatCollaborators(ContributeStatisticsByDay day, ContributionAnalysis.CollaboratorDictionary collaboratorLog)
        {
            IdPairContainerStorage idPairStorage=new IdPairContainerStorage(new TableStorageConnection());
            DbCacheStorage cacheStorage=new DbCacheStorage(new TableStorageConnection(), _dbContext);
            HashSet<string> circleMember = await cacheStorage.GetCircleMemberGitLogins(day.PartitionKey);
            long cumlutiveCount = 0,cumlutiveUnitusCount=0,cumlutiveCircleCount=0,count=0,unitusCount=0,circleCount=0;
            foreach (var loginLangPair in collaboratorLog)
            {
                bool isUnitus = false, isCircle = false;
                long countOfThisCollaborator=0;
                count++;
                if (await idPairStorage.IsStored(loginLangPair.Key, IdPairContainer.GithubLogin, IdPairContainer.UserId))
                {
                    unitusCount++;
                    isUnitus = true;
                }
                if (circleMember.Contains(loginLangPair.Key))
                {
                    circleCount++;
                    isCircle = true;
                }
                foreach (var langCountPair in loginLangPair.Value.LanguageDictionary)
                {
                    countOfThisCollaborator += langCountPair.Value;
                    cumlutiveCount += langCountPair.Value;
                    if (isCircle) cumlutiveCircleCount += langCountPair.Value;
                    if (isUnitus) cumlutiveUnitusCount += langCountPair.Value;
                }
                await _collaboratorCountByDayTable.ExecuteAsync(
                    TableOperation.InsertOrReplace(new CollaboratorCountByDay(day.PartitionKey, loginLangPair.Key,
                        countOfThisCollaborator)));
            }
            day.CumlutiveCollaboratorCount = cumlutiveCount;
            day.CumlutiveCollaboratorCircleMemberCount = cumlutiveCircleCount;
            day.CumlutiveUnitusCollaboratorMemberCount = cumlutiveUnitusCount;
            day.CollaboratorCount = count;
            day.UnitusCollaboratorMemberCount = unitusCount;
            day.CircleCollaboratorMemberCount = circleCount;
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

        /// <summary>
        /// Gist関連のデータを永続化します
        /// </summary>
        /// <param name="gistAnalyzer"></param>
        /// <returns></returns>
        public async Task StoreGistAnalysis(GistAnalyzer gistAnalyzer)
        {
            await
                _gistContributeStatisticsForSingleUser.ExecuteAsync(
                    TableOperation.InsertOrReplace(gistAnalyzer.StatisticsForUser));
            foreach (KeyValuePair<string, GistStatisticsForSingleUserByLanguage> entity in gistAnalyzer.ByLanguages)
            {
                await
                    _gistContributeStatisticsByLanguageForSingleUser.ExecuteAsync(TableOperation.InsertOrReplace(entity.Value));
            }
        }
    }
}