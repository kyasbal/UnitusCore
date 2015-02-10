using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;

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

        public string ValidCronId
        {
            get { return "fsPh5Mj_xg"; }
        }
    }
}
