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
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using System.Web.Helpers;
using Microsoft.Web.Mvc.Resources;
using Newtonsoft.Json;
using Octokit;
using UnitusCore.Controllers.Misc;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;
using UnitusCore.Util;
using UnitusCore.Util.Github;

namespace UnitusCore.Controllers
{
    public class CronJobController : UnitusApiController
    {

        [Route("cron/update/{cronId}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateCircleStatisticsAction(string cronId)
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
        public async Task<IHttpActionResult> ExecuteQueueAction(string cronId)
        {
            if (!cronId.Equals(ValidCronId)) return null;
            var st = new StatisticTaskQueueStorage(new QueueStorageConnection());
            StringBuilder builder=new StringBuilder();
            if (await DequeueTasks(st, builder))
            {//タスクが全くない時
                return Json("All Tasks were completed!");
            }
            return Content(HttpStatusCode.OK, builder.ToString());
        }

        [Route("cron/queue/debug")]
        [HttpPost]
        public async Task<IHttpActionResult> ExecuteDebugQueue()
        {
            var st = new StatisticTaskQueueStorage(new QueueStorageConnection());
            StringBuilder builder = new StringBuilder();
            if (await DequeueTasks(st, builder))
            {//タスクが全くない時
                return Json("All Tasks were completed!");
            }
            if (st.ErrorLogger != null)
            {
                return Content(HttpStatusCode.OK, JsonConvert.SerializeObject(st.ErrorLogger.ToArray(),Formatting.Indented),new RawJsonMediaTypeFormatter(),new MediaTypeHeaderValue("application/json"));
            }
            return Content(HttpStatusCode.OK, builder.ToString());
        }

        private async Task<bool> DequeueTasks(StatisticTaskQueueStorage st, StringBuilder builder)
        {
            return await
                st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.GenerateCacheBeforeStat, 1,
                    async q =>
                    {
                        await
                            ExecuteQueues(q, st);
                    })
                   &&
                   await
                       st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.PreInitializeForGithub, 1,
                           async q =>
                           {
                               await
                                   ExecuteQueues(q, st);
                           })
                   &&
                   await
                       st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.SingleUserContributionStatistics, 5,
                           async q =>
                           {
                               await
                                   ExecuteQueues(q, st);
                           }) 
                   &&
                   await
                       st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.SingleUserAchivementStatistics, 10,
                           async q =>
                           {
                               await
                                   ExecuteQueues(q, st);
                           })
                   &&
                   await
                       st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.CircleAchivementStatistics, 5, 
                           async q =>
                           {
                               await
                                   ExecuteQueues(q, st);
                           })
                   &&
                   await
                       st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.SystemAchivementStatistics,10, 
                           async q =>
                           {
                               await
                                   ExecuteQueues(q, st);
                           })
                   &&
                   await
                       st.CheckNeedOfFinishedTaskExecuteWhenExisiting(builder, StatisticTaskQueueStorage.QueuedTaskType.FinalizeAchivementStatistics,1, 
                           async q =>
                           {
                               await
                                   ExecuteQueues(q, st);
                           });
        }


        private  async Task ExecuteQueues(IEnumerable<StatisticTaskQueueStorage.QueueMessageContainer> tasks, StatisticTaskQueueStorage st)
        {
            foreach (StatisticTaskQueueStorage.QueueMessageContainer q in tasks)
            {
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var argumentObject = System.Web.Helpers.Json.Decode<object>(q.TargetArguments);
                var response = await client.PostAsJsonAsync(Url.Content(q.TargetAddress), argumentObject);
                if (response.IsSuccessStatusCode)
                {
                    st.ErrorLogger.Add(new CronErrorLog(true,q.TaskType, q.TargetAddress, q.TargetArguments, response.StatusCode, response.Headers.ToString(), await response.Content.ReadAsStringAsync()));
                    await st.EndTask(q);
                }
                else
                {
                        st.ErrorLogger.Add(new CronErrorLog(false,q.TaskType,q.TargetAddress,q.TargetArguments,response.StatusCode,response.Headers.ToString(),await response.Content.ReadAsStringAsync()));
                }
            }
        }

        [Route("cron/generate/{cronId}")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateMembersStatisticsQueueAction(string cronId)
        {
            if (!cronId.Equals(ValidCronId)) return null;
            StatisticTaskQueueStorage taskQueueStorage = new StatisticTaskQueueStorage(new QueueStorageConnection());
            AchivementStatisticsStorage acStorage=new AchivementStatisticsStorage(new TableStorageConnection(),DbSession);
            await
    taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.GenerateCacheBeforeStat,
        "/cron/queue/cache", "");
            await
                taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.PreInitializeForGithub,
                    "/cron/queue/preinit/github", "");
            foreach (ApplicationUser user in DbSession.Users)
            {
                await taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.SingleUserContributionStatistics,"/cron/queue/member/stat",
                    new MemberStatisticsArgument(user.Id.ToString(), ValidCronId));
            }
            foreach (ApplicationUser user in DbSession.Users)
            {
                await taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.SingleUserAchivementStatistics, "/cron/queue/member/achivement",
                    new MemberStatisticsArgument(user.Id.ToString(), ValidCronId));
            }
            foreach (Circle circle in DbSession.Circles)
            {
                await taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.CircleAchivementStatistics, "/cron/queue/circle/achivement",
                    new CircleAchivementStatisticsArgument(circle.Id.ToString(), ValidCronId));
            }
            foreach (string achivementName in acStorage.GetAchivementNames())
            {
                await taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.SystemAchivementStatistics, "/cron/queue/system/achivement",
                    new SystemAchivementStatisticsArgument(achivementName, ValidCronId));
            }
            await taskQueueStorage.AddQueue(StatisticTaskQueueStorage.QueuedTaskType.FinalizeAchivementStatistics, "/cron/queue/finalize/achivement","");
            return Json(true);
        }

        [Route("cron/queue/member/stat")]
        [HttpPost]
        public async Task<IHttpActionResult> RunMembersStatisticsAction(MemberStatisticsArgument arg)
        {
            StatisticJobLogStorage jobLog=new StatisticJobLogStorage(new TableStorageConnection());
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.SingleUserStatisticGithubContribution,System.Web.Helpers.Json.Encode(arg));
            try
            {
                GithubAssociationManager associationManager=new GithubAssociationManager(DbSession,UserManager);
                ApplicationUser user = await UserManager.FindByIdAsync(arg.UserId);
                if (await associationManager.IsAssociationEnabled(user))
                {
                    ContributeStatisticsByDay contributeStatistics = ContributeStatisticsByDay.GenerateTodayStatistics(user);
                
                    GitHubClient client = associationManager.GetAuthenticatedClient(user);
                    var st = await associationManager.GetAllRepositoryCommit(client,contributeStatistics);
                    contributeStatistics.SumAddition = st.AdditionCount;
                    contributeStatistics.SumDeletion = st.DeletionCount;
                    contributeStatistics.SumRepository = st.RepositoryCount;
                    contributeStatistics.SumCommit = st.CommitCount;
                    contributeStatistics.SumForked = st.ForkedCount;
                    contributeStatistics.SumForking = st.ForkingCount;
                    contributeStatistics.SumStaring = st.StaredCount;
                    contributeStatistics.SumRepositoryWithOtherComitter = st.RepositoryCountWithOtherCommitter;
                    contributeStatistics.SumRepositoryWithOtherComitterAndAuthority =
                        st.RepositoryCountWithOtherCommitterAndAuthority;
                    ContributeStatisticsByDayStorage storage=new ContributeStatisticsByDayStorage(new TableStorageConnection(),DbSession);
                    await storage.StoreGistAnalysis(await GistAnalyzer.GetGistAnalysis(client, arg.UserId));
                    await storage.Add(contributeStatistics,st.Collaborators);
                }
            }
            catch (RateLimitExceededException rl)
            {
                logger.EndNSync("Failed RateLimitExceeded" + arg.UserId+"\nStackTrace:"+rl);
                return Json(false);
            }
            await logger.End("Success" +arg.UserId);
            return Json(true);
        }

        [Route("cron/queue/member/achivement")]
        [HttpPost]
        public async Task<IHttpActionResult> RunMembersAchivementAction(MemberStatisticsArgument arg)
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.SingleUserAchivementStatistics, System.Web.Helpers.Json.Encode(arg));
            ApplicationUser user = await UserManager.FindByIdAsync(arg.UserId);
            AchivementStatisticsStorage storage=new AchivementStatisticsStorage(tableStorageConnection,DbSession);
            await storage.ExecuteAchivementTask(user);
            await logger.End("Success" + user.Id);
            return Json(true);
        }

        [Route("cron/queue/system/achivement")]
        [HttpPost]
        public async Task<IHttpActionResult> RunSystemAchivementAction(SystemAchivementStatisticsArgument arg)
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.SystemAchivementStatistics, System.Web.Helpers.Json.Encode(arg));
            AchivementStatisticsStorage storage = new AchivementStatisticsStorage(tableStorageConnection,DbSession);
            await storage.ExecuteAchivementStatisticsBySystemTask(arg.AchivementId);
            await logger.End("Success" + arg.AchivementId);
            return Json(true);
        }

        [Route("cron/queue/circle/achivement")]
        [HttpPost]
        public async Task<IHttpActionResult> RunCircleAchivementAction(CircleAchivementStatisticsArgument arg)
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.CircleAchivementStatistics, System.Web.Helpers.Json.Encode(arg));
            AchivementStatisticsStorage storage = new AchivementStatisticsStorage(tableStorageConnection,DbSession);
            Circle circle = await DbSession.Circles.FindAsync(Guid.Parse(arg.CircleId));
            await storage.UpdateCircleStatistics(DbSession, circle);
            await logger.End("Success" + arg.CircleId);
            return Json(true);
        }

        [Route("cron/queue/finalize/achivement")]
        [HttpPost]
        public async Task<IHttpActionResult> RunFinalizeAchivementAction()
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.FinalizeAchivement,"");
            AchivementStatisticsStorage storage = new AchivementStatisticsStorage(tableStorageConnection,DbSession);
            await storage.RemoveAllCachedResult();
            await logger.End("Success achivement finalize");
            return Json(true);
        }

        [Route("cron/queue/preinit/github")]
        [HttpPost]
        public async Task<IHttpActionResult> RunPreInitializeGithubAssociationCheckUserIdPair()
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.InitializeGithubAssociationChrckUserIdPair, "");
            IdPairContainerStorage pairStorage=new IdPairContainerStorage(new TableStorageConnection());
            GithubAssociationManager associationManager=new GithubAssociationManager(DbSession,UserManager);
            HashSet<Person> p=new HashSet<Person>();
            foreach (var people in DbSession.People)
            {
                if (await pairStorage.IsStored(people.Id.ToString(), IdPairContainer.PersonId, IdPairContainer.GithubLogin))
                {
                    continue;
                }
                p.Add(people);
            }
            foreach (Person people in p)
            {
                await people.LoadApplicationUser(DbSession);
                if(!await associationManager.IsAssociationEnabled(people.ApplicationUser))continue;
                var client = associationManager.GetAuthenticatedClient(people.ApplicationUser);
                var githubUser = await client.User.Current();
                await pairStorage.MakePair(people.Id.ToString(), githubUser.Login, IdPairContainer.PersonId, IdPairContainer.GithubLogin);
                await pairStorage.MakePair(people.ApplicationUser.Id, githubUser.Login, IdPairContainer.UserId, IdPairContainer.GithubLogin);
            }
            await DbSession.SaveChangesAsync();
            await logger.End("Success achivement finalize");
            return Json(true);
        }

        [Route("cron/queue/cache")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateCaches()
        {
            var tableStorageConnection = new TableStorageConnection();
            StatisticJobLogStorage jobLog = new StatisticJobLogStorage(tableStorageConnection);
            var logger = jobLog.GetLogger().Begin(DailyStatisticJobAction.GenerateCaches, "");
            DbCacheStorage cacheStorage=new DbCacheStorage(tableStorageConnection,DbSession);
            await cacheStorage.UpdateAllCircles(await DbSession.Circles.ToArrayAsync());
            await logger.End("Success achivement finalize");
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

        public class CircleAchivementStatisticsArgument
        {
            public CircleAchivementStatisticsArgument()
            {
            }

            public CircleAchivementStatisticsArgument(string circleId, string cronId)
            {
                CircleId = circleId;
                CronId = cronId;
            }

            public string CircleId { get; set; }

            public string CronId { get; set; }
        }

        public string ValidCronId
        {
            get { return "fsPh5Mj_xg"; }
        }
    }
}
