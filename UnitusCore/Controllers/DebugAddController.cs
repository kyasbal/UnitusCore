using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;

namespace UnitusCore.Controllers
{
    public class DebugAddController :ApiController
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

        [Route("add/circle")]
        [HttpPost]
        public async Task<IHttpActionResult> AddCircle([FromBody]RequestModel req)
        {
            ApplicationDbContext dbContext = Request.GetOwinContext().Get<ApplicationDbContext>();
            var userManager=Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
           // await userManager.Create()
            //Circle c=new Circle();
            //c.GenerateId();
            //c.Name = req.Name;
            //c.Description = req.Description;
            //c.MemberCount = req.MemberCount;
            //DbSession.Circles.Add(c);
            //DbSession.SaveChanges();
            return Json(true);
        }

        public class RequestModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public int MemberCount { get; set; }
        }
    }
}