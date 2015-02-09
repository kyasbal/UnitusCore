using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
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
  

        public ApplicationDbContext DbSession
        {
            get { return Request.GetOwinContext().Get<ApplicationDbContext>(); }
        }

        public ApplicationUserManager userManager
        {
            get { return Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
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

        [Route("test")]
        [HttpGet]
        public async Task<IHttpActionResult> Test()
        {
            try
            {
                foreach (ApplicationUser user in DbSession.Users.Include(s => s.PersonData))
                {
                    if (user.PersonData == null)
                    {
                        Person person = new Person();
                        person.GenerateId();
                        person.Email = user.Email;
                        DbSession.People.Add(person);
                        user.PersonData = person;
                    }
                }
                DbSession.SaveChanges();
                DbSession.SaveChanges();
            }
            catch (Exception e)
            {
                return Content(HttpStatusCode.OK, e.ToString());
            }
            return Content(HttpStatusCode.OK, "finished");
        }

        public class RequestModel
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public int MemberCount { get; set; }
        }
    }
}