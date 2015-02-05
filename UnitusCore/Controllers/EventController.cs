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
    public class EventController : ApiController
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

        //[Route("event/{eventName}")]
        //public Task<IHttpActionResult> Index(string eventName)
        //{
            
        //}
    }
}