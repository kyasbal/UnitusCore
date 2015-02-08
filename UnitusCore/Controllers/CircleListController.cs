using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using UnitusCore.Attributes;
using UnitusCore.Models;
using UnitusCore.Results;

namespace UnitusCore.Controllers
{
    public class CircleListController : ApiController
    {
        private ApplicationDbContext _dbSession;

        public ApplicationDbContext DbSession
        {
            get
            {
                _dbSession = _dbSession ?? Request.GetOwinContext().Get<ApplicationDbContext>();
                return _dbSession;
            }
        }

        [AllowCrossSiteAccess(AccessFrom.Unitus)]
        [Route("circlelist")]
        [HttpPost]
        public async Task<IHttpActionResult> Index()
        {
            
            Statistics lastStatistics = null;
            var stats = DbSession.Statisticses.Take(20);
            lastStatistics = stats.FirstOrDefault();
            lastStatistics = lastStatistics ?? new Statistics();
            var horizontalLabels = MakeHorizontalLabel(stats, s => s.StatDate.ToString("MM/dd"));
            var verticalLabels = MakeVerticalLabel(stats, stat => stat.SumPeoples.ToString());
            return Json(ResultContainer<Response>.GenerateSuccessResult(new Response
            {
                SumCircleNumber = lastStatistics.SumCircles,
                SumPeople = lastStatistics.SumPeoples,
                GraphPoints = new[] {horizontalLabels, verticalLabels}
            }));
        }

        private string[] MakeVerticalLabel<T>(IEnumerable<T> stats, Func<T, string> injection)
        {
            var labels = new List<string>();
            labels.Add("合計人数");
            foreach (var state in stats)
            {
                labels.Add(injection(state));
            }
            return labels.ToArray();
        }

        private string[] MakeHorizontalLabel<T>(IEnumerable<T> stats, Func<T, string> injection)
        {
            var labels = new List<string>();
            labels.Add("日付");
            foreach (var state in stats)
            {
                labels.Add(injection(state));
            }
            return labels.ToArray();
        }

        [AllowCrossSiteAccess(AccessFrom.All)]
        [Route("circlelist/debug")]
        [HttpPost]
        [HttpGet]
        public async Task<IHttpActionResult> DebugIndex()
        {
            var rand = new Random();
            var times = new List<DateTime>();
            var counts = new List<int>();
            var currentCircleMember = 0;
            var time = DateTime.Now;
            for (var i = 0; i < 20; i++)
            {
                currentCircleMember += rand.Next(0, 100);
                time += new TimeSpan(7, 0, 0, 0);
                times.Add(time);
                counts.Add(currentCircleMember);
            }
            var horizontalLabels = MakeHorizontalLabel(times, s => s.ToString("MM/dd"));
            var verticalLabels = MakeVerticalLabel(counts, s => s.ToString());
            return Json(ResultContainer<Response>.GenerateSuccessResult(new Response
            {
                SumCircleNumber = rand.Next(1, 100),
                SumPeople = currentCircleMember,
                GraphPoints = new[] {horizontalLabels, verticalLabels}
            }));
        }

        private class Response
        {
            public Response()
            {
                GraphPoints = new string[2][];
            }

            public int SumCircleNumber { get; set; }

            public int SumPeople { get; set; }

            public string[][] GraphPoints { get; set; }
        }
    }
}
