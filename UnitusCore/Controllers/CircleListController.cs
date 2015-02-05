using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    
    public class CircleListController : ApiController
    {
        private BasicDbContext _dbSession;

        public BasicDbContext DbSession
        {
            get
            {
                _dbSession = _dbSession ?? Request.GetOwinContext().Get<BasicDbContext>();
                return _dbSession;
            }
        }


        [Route("circlelist")]
        [HttpPost]
        public async Task<IHttpActionResult> Index()
        {
            Statistics lastStatistics = null;
            var stats = DbSession.Statisticses.Take(20);
            List<SamplePoint> result=new List<SamplePoint>();
            foreach (var stat in stats)
            {
                if (lastStatistics == null)
                {
                    lastStatistics = stat;
                }
                result.Add(SamplePoint.FromStatistics(stat));
            }
            lastStatistics = lastStatistics ?? new Statistics();//NULLにはならないはずだが、対策。
            return Json(ResultContainer<Response>.GenerateSuccessResult(new Response()
            {
                SumCircleNumber = lastStatistics.SumCircles,
                SumPeople = lastStatistics.SumPeoples,
                GraphPoints = result.ToArray()
            }));
        }

        [Route("circlelist/debug")]
        [HttpPost]
        public async Task<IHttpActionResult> DebugIndex()
        {
            Random rand=new Random();
            List<SamplePoint> points=new List<SamplePoint>();
            int currentCircleMember = 0;
            DateTime time = DateTime.Now;
            for (int i = 0; i < 20; i++)
            {
                currentCircleMember += rand.Next(0, 100);
                time += new TimeSpan(7, 0, 0, 0);
                points.Add(new SamplePoint() {SumPeople = currentCircleMember,UnixTime = time.ToUnixTime()});
            }
            return Json(ResultContainer<Response>.GenerateSuccessResult(new Response()
            {
                SumCircleNumber = rand.Next(1,100),
                SumPeople = currentCircleMember,
                GraphPoints = points.ToArray()
            }));
        }

        class Response
        {
            public int SumCircleNumber { get; set; }

            public int SumPeople { get; set; }
            
            public SamplePoint[] GraphPoints { get; set; } 
        }

        class SamplePoint
        {

            public static SamplePoint FromStatistics(Statistics stat)
            {
                SamplePoint p=new SamplePoint();
                p.UnixTime = stat.StatDate.ToUnixTime();
                p.SumPeople = stat.SumPeoples;
                return p;
            }
             public long UnixTime { get; set; }

            public int SumPeople { get; set; }
        }

    }
}
