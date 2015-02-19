using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using System.Web.Helpers;
using Microsoft.Web.Mvc.Resources;
using Octokit;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CronJobController : UnitusApiController
    {

        [Route("cron/update/{cronId}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCircleStatistics(string cronId)
        {
            if (cronId !=ValidCronId) return null;
            int circleSum = 0;
            int peopleSum = 0;
            foreach (var circle in DbSession.Circles)
            {
                circleSum++;
                peopleSum += circle.MemberCount;
            }
            Statistics st=new Statistics();
            st.GenerateId();
            st.StatDate = DateTime.Now;
            st.SumCircles = circleSum;
            st.SumPeoples = peopleSum;
            DbSession.Statisticses.Add(st);
            DbSession.SaveChanges();
            return Json(true);
        }

        [Route("cron/queue/{cronId}")]
        [HttpPost]
        public async Task<IHttpActionResult> ExecuteQueue(string cronId)
        {
            if (!cronId.Equals(ValidCronId)) return null;
            var st = new StatisticTaskQueueStorage(new QueueStorageConnection());
            if (
                await
                    st.CheckNeedOfFinishedTaskExecuteWhenExisiting(
                        StatisticTaskQueueStorage.QueuedTaskType.SingleUserContributionStatistics, 5,
                        async q =>
                        {
                            await
                                ExecuteQueues(q, st);
                        }) 
                        &&
                await
                    st.CheckNeedOfFinishedTaskExecuteWhenExisiting(
                        StatisticTaskQueueStorage.QueuedTaskType.SingleUserAchivementStatistics, 10,
                        async q =>
                        {
                            await
                                ExecuteQueues(q, st);
                        })
                        &&
                await
                    st.CheckNeedOfFinishedTaskExecuteWhenExisiting(
                        StatisticTaskQueueStorage.QueuedTaskType.SystemAchivementStatistics,10, async q =>
                        {
                            await
                                ExecuteQueues(q, st);
                        }))
            {//タスクが全くない時
                return Json("All Tasks were completed!");
            }
            return Json(true);
        }

        private static async Task ExecuteQueues(IEnumerable<StatisticTaskQueueStorage.QueueMessageContainer> tasks, StatisticTaskQueueStorage st)
        {
            foreach (StatisticTaskQueueStorage.QueueMessageContainer q in tasks)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var argumentObject = System.Web.Helpers.Json.Decode<object>(q.TargetArguments);
                var response = await client.PostAsJsonAsync(q.TargetAddress, argumentObject);
                if (response.IsSuccessStatusCode)
                {
                    await st.EndTask(q);
                }
            }
        }

        [Route("cron/generate/{cronId}")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateMembersStatisticsQueue(string cronId)
        {
            if (!cronId.Equals(ValidCronId)) return null;
            StatisticTaskQueueStorage taskQueueStorage = new StatisticTaskQueueStorage(new QueueStorageConnection());
            AchivementStatisticsStorage acStorage=new AchivementStatisticsStorage(new TableStorageConnection());
            foreach (ApplicationUser user in DbSession.Users)
            {
                StatisticTaskQueueStorage queueStorage=taskQueueStorage;
                await queueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.SingleUserContributionStatistics,Url.Content("/cron/queue/member/stat"),
                    new MemberStatisticsArgument(user.Id.ToString(), ValidCronId));
            }
            foreach (ApplicationUser user in DbSession.Users)
            {
                StatisticTaskQueueStorage queueStorage = taskQueueStorage;
                await queueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.SingleUserAchivementStatistics, Url.Content("/cron/queue/member/achivement"),
                    new MemberStatisticsArgument(user.Id.ToString(), ValidCronId));
            }
            foreach (string achivementName in acStorage.GetAchivementNames())
            {
                StatisticTaskQueueStorage queueStorage = taskQueueStorage;
                await queueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.SystemAchivementStatistics, Url.Content("/cron/queue/system/achivement"),
                    new SystemAchivementStatisticsArgument(achivementName, ValidCronId));
            }
            return Json(true);
        }

        [Route("cron/queue/member/stat")]
        [HttpPost]
        public async Task<IHttpActionResult> RunMembersStatistics(MemberStatisticsArgument arg)
        {
            StatisticJobLogStorage jobLog=new StatisticJobLogStorage(new TableStorageConnection());
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.SingleUserStatisticGithubContribution,System.Web.Helpers.Json.Encode(arg));
            GithubAssociationManager associationManager=new GithubAssociationManager(DbSession,UserManager);
            ApplicationUser user = await UserManager.FindByIdAsync(arg.UserId);
            if (await associationManager.IsAssociationEnabled(user))
            {
                ContributeStatisticsByDay contributeStatistics = ContributeStatisticsByDay.GenerateTodayStatistics(user);
                
                GitHubClient client = associationManager.GetAuthenticatedClient(user);
                var st = await associationManager.GetAllRepositoryCommit(client,contributeStatistics);
                contributeStatistics.SumAddition = st.SumAddition;
                contributeStatistics.SumDeletion = st.SumDeletion;
                contributeStatistics.SumRepository = st.RepositoryCount;
                contributeStatistics.SumCommit = st.SumCommit;
                ContributeStatisticsByDayStorage storage=new ContributeStatisticsByDayStorage(new TableStorageConnection());
                await storage.Add(contributeStatistics);
            }
            await logger.End("Success" + user.Id);
            return Json(true);
        }

        [Route("cron/queue/member/achivement")]
        [HttpPost]
        public async Task<IHttpActionResult> RunMembersAchivement(MemberStatisticsArgument arg)
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.SingleUserAchivementStatistics, System.Web.Helpers.Json.Encode(arg));
            ApplicationUser user = await UserManager.FindByIdAsync(arg.UserId);
            AchivementStatisticsStorage storage=new AchivementStatisticsStorage(tableStorageConnection);
            await storage.ExecuteAchivementTask(user);
            await logger.End("Success" + user.Id);
            return Json(true);
        }

        [Route("cron/queue/system/achivement")]
        [HttpPost]
        public async Task<IHttpActionResult> RunSystemAchivement(SystemAchivementStatisticsArgument arg)
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.SystemAchivementStatistics, System.Web.Helpers.Json.Encode(arg));
            AchivementStatisticsStorage storage = new AchivementStatisticsStorage(tableStorageConnection);
            await storage.ExecuteAchivementStatisticsBySystemTask(arg.AchivementId);
            await logger.End("Success" + arg.AchivementId);
            return Json(true);
        }

        public class MemberStatisticsArgument
        {
            public MemberStatisticsArgument()
            {
            }

            public MemberStatisticsArgument(string userId, string cronId)
            {
                UserId = userId;
                CronId = cronId;
            }

            public string UserId { get; set; }

            public string CronId { get; set; }
        }


        public class SystemAchivementStatisticsArgument
        {
            public SystemAchivementStatisticsArgument()
            {
            }

            public SystemAchivementStatisticsArgument(string achivementId, string cronId)
            {
                AchivementId = achivementId;
                CronId = cronId;
            }

            public string AchivementId { get; set; }

            public string CronId { get; set; }
        }

        public string ValidCronId
        {
            get { return "fsPh5Mj_xg"; }
        }
    }
}
