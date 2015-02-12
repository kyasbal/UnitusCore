using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Attributes;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleController : UnitusApiController
    {

        private ApplicationUser GetWithAdministrationCircle
        {
            get
            {
                if (currentUserCache == null || currentUserCache.AdministrationCircle == null)
                {
                    currentUserCache =
                        DbSession.Users.Include(a => a.PersonData)
                            .Include(a => a.AdministrationCircle)
                            .Where(a => a.UserName.Equals(User.Identity.Name))
                            .FirstOrDefault();
                }
                return currentUserCache;
            }
        }

        [EnableCors(GlobalConstants.CorsOrigins, "*", "*")]
        [HttpGet]
        [Route("Circle")]
        [Authorize]
        public async Task<IHttpActionResult> GetCircle(string validationToken, int Count = 20, int Offset = 0)
        {
            return await this.OnValidToken(validationToken, () =>
            {
                DbSession.Entry(CurrentUserWithPerson.PersonData).Collection<MemberStatus>(a => a.BelongedCircles).Load();
                HashSet<Circle> resultCircle=new HashSet<Circle>();
                HashSet<Circle> circleBelonging=new HashSet<Circle>();
                foreach (MemberStatus belongedCircles in CurrentUserWithPerson.PersonData.BelongedCircles)
                {
                    DbSession.Entry(belongedCircles).Reference<Circle>(a => a.TargetCircle).Load();
                    circleBelonging.Add(belongedCircles.TargetCircle);
                }//とりあえず自分の入っているサークルをHashSetに入れておく。
                if (Offset > resultCircle.Count)
                {
                    Offset -= circleBelonging.Count;
                }
                else
                {
                    foreach (var circle in circleBelonging)
                    {
                        if (Offset > 0)
                        {
                            Offset--;
                            continue;
                        }
                        else
                        {
                            resultCircle.Add(circle);
                            Count--;
                        }
                    }
                }
                List<GetCircleResponseCircleEntity> responseData=new List<GetCircleResponseCircleEntity>();
                responseData.AddRange(resultCircle.Select(a=>GetCircleResponseCircleEntity.FromCircle(a,true)));
                resultCircle.Clear();
                //残りは自分の入っていない中からとってくる。
                var taken=DbSession.Circles.OrderBy(c => c.Name).Skip(Offset).Take(Count + circleBelonging.Count);
                foreach (Circle circle in taken)
                {
                    if (Count == 0) break;
                    if (!circleBelonging.Contains(circle))
                    {
                        Count--;
                        resultCircle.Add(circle);
                    }
                }
                responseData.AddRange(resultCircle.Select(a => GetCircleResponseCircleEntity.FromCircle(a,false)));
                return Json(ResultContainer<GetCircleResponse>.GenerateSuccessResult(new GetCircleResponse(responseData.ToArray())));
            });
        }

        [EnableCors(GlobalConstants.CorsOrigins, "*", "*")]
        [HttpGet]
        [Route("Circle/Dummy")]
        public async Task<IHttpActionResult> GetCircleDummy(string validationToken, int Count=20, int Offset=0)
        {
            return await this.OnValidToken(validationToken, () =>
            {
                Random rand=new Random();
                int belongedCount = rand.Next(Count);
                List<GetCircleResponseCircleEntity> responseData=new List<GetCircleResponseCircleEntity>();
                for (int i = 0; i < belongedCount; i++)
                {
                    DateTime randomTime = DateTime.Now - new TimeSpan(rand.Next(10000),0, 0, 0);
                    responseData.Add(new GetCircleResponseCircleEntity(IdGenerator.GetId(10),rand.Next(120),IdGenerator.GetId(10)+"大学",randomTime.ToString("yyyy年M月d日"),true,Guid.NewGuid().ToString()));
                }
                for (int i = 0; i < Count-belongedCount; i++)
                {
                    DateTime randomTime = DateTime.Now - new TimeSpan(rand.Next(10000), 0, 0, 0);
                    responseData.Add(new GetCircleResponseCircleEntity(IdGenerator.GetId(10), rand.Next(120), IdGenerator.GetId(10) + "大学", randomTime.ToString("yyyy年M月d日"), false, Guid.NewGuid().ToString()));
                }
                return Json(ResultContainer<GetCircleResponse>.GenerateSuccessResult(new GetCircleResponse(responseData.ToArray())));
            });
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
                    ApplicationUser user = CurrentUserWithPerson;
               
                    MemberStatus memberStatus = new MemberStatus();
                    memberStatus.GenerateId();
                    memberStatus.IsActiveMember = true;
                    memberStatus.Occupation = "代表者";
                    memberStatus.TargetUser = user.PersonData;
                    Circle circle = new Circle();
                    memberStatus.TargetCircle = circle;
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
                    DbSession.MemberStatuses.Add(memberStatus);
                    DbSession.Circles.Add(circle);
                    DbSession.SaveChanges();
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
        public GetCircleResponse(GetCircleResponseCircleEntity[] circle)
        {
            Circle = circle;
        }

        public GetCircleResponseCircleEntity[] Circle { get; set; }

    }

    public class GetCircleResponseCircleEntity
    {
        public static GetCircleResponseCircleEntity FromCircle(Circle circle,bool isBelonging)
        {
            return new GetCircleResponseCircleEntity(circle.Name,circle.MemberCount,circle.BelongedSchool,circle.LastModefied.ToString("yyyy年M月d日"),isBelonging,circle.Id.ToString());
        }

        public GetCircleResponseCircleEntity(string circleName, int memberCount, string belongedUniversity, string lastUpdateDate, bool isBelonged, string circleId)
        {
            CircleName = circleName;
            MemberCount = memberCount;
            BelongedUniversity = belongedUniversity;
            this.LastUpdateDate = lastUpdateDate;
            IsBelonging = isBelonged;
            CircleId = circleId;
        }

        public string CircleId { get; set; }

        public bool IsBelonging { get; set; }

        public string CircleName { get; set; }

        public int MemberCount { get; set; }

        public string BelongedUniversity { get; set; }

        public string LastUpdateDate { get; set; }
    }
}
