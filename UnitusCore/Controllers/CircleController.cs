using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Attributes;
using UnitusCore.Models;
using UnitusCore.Results;

namespace UnitusCore.Controllers
{
    public class CircleController : ApiController
    {
        public ApplicationUserManager UserManager
        {
            get { return Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        public IAuthenticationManager AuthenticationContext
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        public ApplicationDbContext ApplicationDbSession
        {
            get { return Request.GetOwinContext().Get<ApplicationDbContext>(); }
        }

        public BasicDbContext BasicDbSession
        {
            get { return Request.GetOwinContext().Get<BasicDbContext>(); }
        }

        [HttpPost]
        [Authorize]
        [RoleRestrict("Administrator")]
        [Route("Circle")]
        public Task<IHttpActionResult> AddCircle(AddCircleRequest req)
        {
            return this.OnValidToken(req, (r) =>
            {
                try
                {
                    ApplicationUser user = UserManager.FindByName(req.LeaderUserName);
                    //MemberStatus memberStatus=new MemberStatus();
                    //memberStatus.GenerateId();
                    //memberStatus.IsActiveMember = true;
                    //memberStatus.Occupation = "代表者";
                    //memberStatus.TargetPerson = user.AccessablePerson;
                    //Circle circle = new Circle();
                    //circle.GenerateId();
                    //circle.Name = r.CircleName;
                    //circle.Description = r.Description;
                    //circle.MemberCount = r.MemberCount;
                    //circle.WebAddress = r.WebSiteAddress;
                    //circle.BelongedSchool = r.BelongedUniversity;
                    //circle.Notes = r.Note;
                    //circle.Contact = r.Address;
                    //circle.CanInterCollege = r.InterColledgeAccepted;
                    //circle.Members.Add(user);
                    //BasicDbSession.Circles.Add(circle);
                    //BasicDbSession.SaveChanges();
                    //return Json(ResultContainer.GenerateSuccessResult());
                }
                catch (Exception exe)
                {
                    return Json(ResultContainer.GenerateFaultResult("LeaderId notfound"));
                }

                return Json(true);
            });
        }

        [HttpPatch]
        [Authorize]
        [Route("Circle")]
        public Task<IHttpActionResult> PatchCircle(AddCircleRequest req)
        {
            return this.OnValidToken(req, (r) =>
            {
                return Json(true);
            });
        }

    }

    public class AddCircleRequest :AjaxRequestModelBase
    {
        public string CircleName { get; set; }

        public string Description { get; set; }

        public int MemberCount { get; set; }

        public string WebSiteAddress { get; set; }

        public string BelongedUniversity { get; set; }

        public string Note { get; set; }

        public string Address { get; set; }

        public string LeaderUserName { get; set; }

        public bool InterColledgeAccepted { get; set; }
    }
}
