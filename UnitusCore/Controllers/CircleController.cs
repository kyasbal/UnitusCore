using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        //[AllowCrossSiteAccess(AccessFrom.All)]
        //[HttpGet]
        //[Route("Circle")]
        //[Authorize]
        //public async Task<IHttpActionResult> GetCircle(string validationToken, int Count=20,int Offset=0)
        //{
        //    return await this.OnValidToken(validationToken, () =>
        //    {
                
        //    });
        //}

        //[HttpGet]
        //[Route("Circle/Dummy")]
        //public async Task<IHttpActionResult> GetCircleDummy(string validationToken, int Count,int Offset)
        //{
        //    return await this.OnValidToken(validationToken, () =>
        //    {
                
        //    });
        //}


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
                    MemberStatus memberStatus = new MemberStatus();
                    memberStatus.GenerateId();
                    memberStatus.IsActiveMember = true;
                    memberStatus.Occupation = "代表者";
                    memberStatus.TargertUserKey = Guid.Parse(user.Id);
                    ApplicationDbSession.MemberStatuses.Add(memberStatus);
                    ApplicationDbSession.SaveChanges();
                    Circle circle = new Circle();
                    circle.GenerateId();
                    circle.Name = r.CircleName;
                    circle.Description = r.Description;
                    circle.MemberCount = r.MemberCount;
                    circle.WebAddress = r.WebSiteAddress;
                    circle.BelongedSchool = r.BelongedUniversity;
                    circle.Notes = r.Note;
                    circle.Contact = r.Address;
                    circle.CanInterCollege = r.InterColledgeAccepted;
                    circle.Members.Add(memberStatus);
                    ApplicationDbSession.Circles.Add(circle);
                    ApplicationDbSession.SaveChanges();
                    return Json(ResultContainer.GenerateSuccessResult());
                }
                catch (Exception exe)
                {
                    Trace.WriteLine(exe.ToString());
                    return Json(ResultContainer.GenerateFaultResult(exe.ToString()));
                }
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

    public class GetCircleResponse
    {
        public GetCircleResponse(GetPersonListPersonEntity[] circle)
        {
            Circle = circle;
        }

        public GetPersonListPersonEntity[] Circle { get; set; }

    }

    public class GetCircleResponseCircleEntity
    {
        public GetCircleResponseCircleEntity(string circleName, int memberCount, string belongedUniversity, string lastUpdateDate, bool isBelonged)
        {
            CircleName = circleName;
            MemberCount = memberCount;
            BelongedUniversity = belongedUniversity;
            this.LastUpdateDate = lastUpdateDate;
            IsBelonged = isBelonged;
        }
        public bool IsBelonged { get; set; }

        public string CircleName { get; set; }

        public int MemberCount { get; set; }

        public string BelongedUniversity { get; set; }

        public string LastUpdateDate { get; set; }
    }
}
