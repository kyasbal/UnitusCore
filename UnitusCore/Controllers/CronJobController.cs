using System;
using System.Collections.Generic;
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
            HashSet<CronQueue> executeQueue=new HashSet<CronQueue>();
            foreach (CronQueue queue in DbSession.CronjobQueue.OrderBy(a=>a.QueueTime).Take(5))
            {
                executeQueue.Add(queue);
            }
            Parallel.ForEach(executeQueue,async (q) =>
            {
                Stopwatch w=new Stopwatch();
                w.Start();
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var argumentObject = System.Web.Helpers.Json.Decode<object>(q.Arguments);
                var response = await client.PostAsJsonAsync(q.TargetAddress, argumentObject);
                w.Stop();
                CronQueueLog log = new CronQueueLog(DateTime.Now, q.Arguments, q.TargetAddress, w.ElapsedMilliseconds,
                    await response.Content.ReadAsStringAsync());
                DbSession.CronjobQueueLog.Add(log);
            });
            await DbSession.SaveChangesAsync();
            return Json(true);
        }

        [Route("cron/generate/{cronId}")]
        [HttpPost]
        public async Task<IHttpActionResult> GenerateMembersStatisticsQueue(string cronId)
        {
            if (!cronId.Equals(ValidCronId)) return null;
            foreach (ApplicationUser user in DbSession.Users)
            {
                CronQueue queue=new CronQueue();
                queue.GenerateId();
                queue.TargetAddress = Url.Content("/cron/queue/member/stat");
                queue.Arguments = System.Web.Helpers.Json.Encode(new MemberStatisticsArgument(user.Id.ToString(),ValidCronId));
                queue.QueueTime = DateTime.Now;
                DbSession.CronjobQueue.Add(queue);
            }
            await DbSession.SaveChangesAsync();
            return Json(true);
        }

        [Route("cron/queue/member/stat")]
        [HttpPost]
        public async Task<IHttpActionResult> RunMembersStatistics(MemberStatisticsArgument arg)
        {
            GithubAssociationManager associationManager=new GithubAssociationManager(DbSession,UserManager);
            UserStatistics statistics=new UserStatistics();
            statistics.GenerateId();
            statistics.RecordedTime = DateTime.Now;
            statistics.LinkedPerson = CurrentUserWithPerson.PersonData;
            
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

        public string ValidCronId
        {
            get { return "fsPh5Mj_xg"; }
        }
    }
}
