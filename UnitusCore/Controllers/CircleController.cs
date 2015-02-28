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
using AutoMapper;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Attributes;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleController : UnitusApiController
    {
        #region Dummy

        [UnitusCorsEnabled]
        [Route("Circle/Dummy")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCircleDummy(string validationToken, int Count = 20, int Offset = 0)
        {
            return await this.OnValidToken(validationToken, () =>
            {
                Random rand = new Random();
                int belongedCount = rand.Next(Count);
                List<GetCircleResponseCircleEntity> responseData = new List<GetCircleResponseCircleEntity>();
                for (int i = 0; i < belongedCount; i++)
                {
                    DateTime randomTime = DateTime.Now - new TimeSpan(rand.Next(10000), 0, 0, 0);
                    responseData.Add(new GetCircleResponseCircleEntity(IdGenerator.GetId(10), rand.Next(120),
                        IdGenerator.GetId(10) + "大学", randomTime.ToString("yyyy年M月d日"), true, Guid.NewGuid().ToString()));
                }
                for (int i = 0; i < Count - belongedCount; i++)
                {
                    DateTime randomTime = DateTime.Now - new TimeSpan(rand.Next(10000), 0, 0, 0);
                    responseData.Add(new GetCircleResponseCircleEntity(IdGenerator.GetId(10), rand.Next(120),
                        IdGenerator.GetId(10) + "大学", randomTime.ToString("yyyy年M月d日"), false, Guid.NewGuid().ToString()));
                }
                return
                    Json(
                        ResultContainer<GetCircleResponse>.GenerateSuccessResult(
                            new GetCircleResponse(responseData.ToArray())));
            });
        }


        [UnitusCorsEnabled]
        [Route("Circle/Detail/Dummy")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCircleDetailedDummy(string validationToken, string circleId)
        {
            return Json(ResultContainer<GetPutCircleDetailBody>.GenerateSuccessResult(new GetPutCircleDetailBody(
                "応用数学研究部", "# 数学とかwwww", 28, "Tokyo university of science,Kagura-zaka", "特になし", "http://unitus-ac.com",
                "なし", false, "毎週木曜日")
                ));
        }

        #endregion

        [UnitusCorsEnabled]
        [HttpDelete]
        [Route("Circle")]
        [ApiAuthorized]
        [RoleRestrict(GlobalConstants.AdminRoleName)]
        public async Task<IHttpActionResult> RemoveCircle(DeleteCircleRequest req)
        {
            return await this.OnValidToken(req.ValidationToken, async () =>
            {
                var circle = await Ensure.ExisitingCircleId(req.CircleId);
                DbSession.Circles.Remove(circle);
                await DbSession.SaveChangesAsync();
                return Json(ResultContainer.GenerateSuccessResult());
            });
        }


        [UnitusCorsEnabled]
        [HttpGet]
        [Route("Circle")]
        [ApiAuthorized]
        public async Task<IHttpActionResult> GetCircle(string validationToken, int Count = 20, int Offset = 0)
        {
            return await this.OnValidToken(validationToken,async () =>
            {
                await CurrentUserWithPerson.PersonData.LoadBelongingCircles(DbSession);
                HashSet<Circle> resultCircle = new HashSet<Circle>();
                HashSet<Circle> circleBelonging = new HashSet<Circle>();
                foreach (MemberStatus belongedCircles in CurrentUserWithPerson.PersonData.BelongedCircles)
                {
                    await belongedCircles.LoadReferencesAsync(DbSession);
                    circleBelonging.Add(belongedCircles.TargetCircle);
                } //とりあえず自分の入っているサークルをHashSetに入れておく。
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
                List<GetCircleResponseCircleEntity> responseData = new List<GetCircleResponseCircleEntity>();
                responseData.AddRange(resultCircle.Select(a => GetCircleResponseCircleEntity.FromCircle(a, true)));
                resultCircle.Clear();
                //残りは自分の入っていない中からとってくる。
                var taken = DbSession.Circles.OrderBy(c => c.Name).Skip(Offset).Take(Count + circleBelonging.Count);
                foreach (Circle circle in taken)
                {
                    if (Count == 0) break;
                    if (!circleBelonging.Contains(circle))
                    {
                        Count--;
                        resultCircle.Add(circle);
                    }
                }
                responseData.AddRange(resultCircle.Select(a => GetCircleResponseCircleEntity.FromCircle(a, false)));
                return
                    Json(
                        ResultContainer<GetCircleResponse>.GenerateSuccessResult(
                            new GetCircleResponse(responseData.ToArray())));
            });
        }

        [UnitusCorsEnabled]
        [Route("Circle/Detail")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCircleDetailed(string validationToken, string circleId)
        {
            Circle circle = await Ensure.ExisitingCircleId(circleId);
            return await this.OnValidToken(validationToken, () =>
            {
                return
                    Json(
                        ResultContainer<GetPutCircleDetailBody>.GenerateSuccessResult(
                            GetPutCircleDetailBody.FromCircle(circle)));
            });
        }

        private bool CheckCircleExisiting(string circleName,string belongedTo)
        {
            return DbSession.Circles.Where(a => a.Name.Equals(circleName) && a.BelongedSchool.Equals(belongedTo)).Any();
        }


        [HttpPost]
        [ApiAuthorized]
        [RoleRestrict("Administrator")]
        [Route("Circle")]
        public Task<IHttpActionResult> AddCircle(AddCircleRequest req)
        {
            ApplicationUser user = null;
            return this.OnValidToken(req, async (r) =>
            {
                try
                {
                    MemberStatus memberStatus = new MemberStatus();
                    memberStatus.GenerateId();
                    memberStatus.IsActiveMember = true;
                    memberStatus.Occupation = "代表者";
                    memberStatus.TargetUser = user.PersonData;
                    Circle circle = new Circle();
                    await circle.LoadAdministrators(DbSession);
                    await circle.LoadMembers(DbSession);
                    circle.Administrators.Add(user);
                    await user.LoadAdministrationCircles(DbSession);
                    user.AdministrationCircle.Add(circle);
                    memberStatus.TargetCircle = circle;
                    circle.GenerateId();
                    circle.Members.Add(memberStatus);
                    Mapper.Map(req, circle);
                    DbSession.MemberStatuses.Add(memberStatus);
                    DbSession.Circles.Add(circle);
                    DbSession.SaveChanges();
                    return await GetCircleDetailed(req.ValidationToken, circle.Id.ToString());
                }
                catch (Exception exe)
                {
                    Trace.WriteLine(exe.ToString());
                    return Json(ResultContainer.GenerateFaultResult(exe.ToString()));
                }
            },async (r,set) =>
            {
                if (CheckCircleExisiting(req.Name, req.BelongedSchool))
                {
                    set.Add("その団体は既に存在します。");
                    return false;
                }
                user = await UserManager.FindByNameAsync(req.LeaderUserName);
                
                if (user == null)
                {
                    set.Add("ユーザーIDが存在しません。");
                    return false;
                }
                await user.LoadPersonData(DbSession);
                return true;
            });
        }

        [CircleMember("circleId")]
        [HttpGet]
        [Route("Circle/Member")]
        [ApiAuthorized]
        public async Task<IHttpActionResult> GetMembers(string validationToken, string circleId)
        {
            Circle circle = await Ensure.ExisitingCircleId(circleId);
            bool authority = await circle.HaveAuthority(CurrentUser, DbSession);
            CircleTagStorage stroage=new CircleTagStorage(new TableStorageConnection());
            await circle.LoadMembers(DbSession);
            List<GetMemberListElement> elements = new List<GetMemberListElement>();
            foreach (MemberStatus members in circle.Members)
            {
                await members.LoadReferencesAsync(DbSession);
                await members.TargetUser.LoadApplicationUser(DbSession);
                GetMemberListElement memberElement = new GetMemberListElement(members.TargetUser.ApplicationUser.Id, members.TargetUser.Name,
                    members.Occupation, members.TargetUser.CurrentCource, members.IsActiveMember,members.TargetUser.BelongedColledge,members.TargetUser.Faculty,members.TargetUser.Major);
                memberElement.Tags =
                    await stroage.GetAppliedTags(circle.Id.ToString(), members.TargetUser.ApplicationUser.Id,authority);
                elements.Add(memberElement);
            }
            return Json(ResultContainer<GetMemberList>.GenerateSuccessResult(new GetMemberList(elements.ToArray())));
        }

        [CircleAdmin("circleId")]
        [HttpPut]
        [Route("Circle/Member")]
        [ApiAuthorized]
        public async Task<IHttpActionResult> PutMembersState(PutMemberStateRequest req)
        {
            return await this.OnValidToken(req,async (r) =>
            {
                Circle circle = await Ensure.ExisitingCircleId(req.CircleId);
                Person targetUser = await Ensure.ExistingPersonId(req.PersonId);
                await circle.LoadMembers(DbSession);
                bool found = false;
                foreach (MemberStatus memberStatus in circle.Members)
                {
                    await memberStatus.LoadReferencesAsync(DbSession);
                    if (memberStatus.TargetUser.Id.Equals(targetUser.Id))
                    {
                        memberStatus.IsActiveMember = r.IsActiveMember;
                        memberStatus.Occupation =r.Ocupation;
                        found = true;
                        break;
                    }
                }
                if (!found) return Json(ResultContainer.GenerateFaultResult("指定したユーザーがサークル内に見つかりません。"));
                await DbSession.SaveChangesAsync();
                return Json(ResultContainer.GenerateSuccessResult());
            });


        }



        [HttpPut]
        [ApiAuthorized]
        [Route("Circle")]
        public async Task<IHttpActionResult> PutCircle(PutCircleRequest req)
        {
            Circle circle = await Ensure.ExisitingCircleId(req.CircleId);
            return await this.OnValidToken(req.ValidationToken, async () =>
            {
                circle.Name = req.CircleName ?? circle.Name;
                circle.Description = req.CircleDescription ?? circle.Description;
                circle.MemberCount = req.MemberCount != 0 ? req.MemberCount : circle.MemberCount;
                circle.WebAddress = req.WebAddress ?? circle.WebAddress;
                circle.BelongedSchool = req.BelongedSchool ?? circle.BelongedSchool;
                circle.Notes = req.Notes ?? circle.Notes;
                circle.Contact = req.Contact ?? circle.Contact;
                circle.ActivityDate = req.ActivityDate ?? circle.ActivityDate;
                circle.CanInterColledge = req.CanInterColledge;

                DbSession.SaveChanges();
                return await GetCircleDetailed(req.ValidationToken, req.CircleId);
            });
        }

        public class PutMemberStateRequest:AjaxRequestModelBase
        {
            public string CircleId { get; set; }

            public string PersonId { get; set; }

            public string Ocupation { get; set; }

            public bool IsActiveMember { get; set; }
        }


        public class GetMemberList
        {
            public GetMemberList(GetMemberListElement[] members)
            {
                Members = members;
            }

            public GetMemberListElement[] Members { get; set; }
        }

        public class GetMemberListElement
        {
            public GetMemberListElement(string userId, string name, string ocupation, Person.Cource currentGrade,
                bool isActiveMember, string belongedUniversity, string faculty, string major)
            {
                UserId = userId;
                Ocupation = ocupation;
                CurrentGrade = currentGrade;
                IsActiveMember = isActiveMember;
                BelongedUniversity = belongedUniversity;
                Faculty = faculty;
                Major = major;
                Name = name;
            }

            public string UserId { get; set; }

            public string Ocupation { get; set; }

            public Person.Cource CurrentGrade { get; set; }

            public bool IsActiveMember { get; set; }

            public string Name { get; set; }

            public string BelongedUniversity { get; set; }

            public string Faculty { get; set; }

            public string Major { get; set; }

            public IEnumerable<string> Tags { get; set; } 

        }


        public class AddCircleRequest : AjaxRequestModelBase,ICircleInfoContainer
        {
            
            public string Name { get; set; }

            public string Description { get; set; }

            public int MemberCount { get; set; }

            public string WebAddress { get; set; }

            public string BelongedSchool { get; set; }

            public string Notes { get; set; }

            public string Contact { get; set; }

            public string LeaderUserName { get; set; }

            public bool CanInterColledge { get; set; }

            public string ActivityDate { get; set; }
        }

        public class DeleteCircleRequest
        {
            public string CircleId { get; set; }
            public string ValidationToken { get; set; }
        }

        public class PutCircleRequest : GetPutCircleDetailBody
        {
            public string CircleId { get; set; }
            public string ValidationToken { get; set; }

            public PutCircleRequest(string circleId, string validationToken, string circleName, string circleDescription,
                int memberCount, string belongedSchool, string notes, string webAddress, string contact,
                bool canInterColledge, string activityDate)
                : base(
                    circleName, circleDescription, memberCount, belongedSchool, notes, webAddress, contact,
                    canInterColledge, activityDate)
            {
                CircleId = circleId;
                ValidationToken = validationToken;
            }

            public PutCircleRequest()
            {

            }
        }

        public class GetPutCircleDetailBody
        {
            public GetPutCircleDetailBody()
            {

            }

            public GetPutCircleDetailBody(string circleName, string circleDescription, int memberCount,
                string belongedSchool, string notes, string webAddress, string contact, bool canInterColledge,
                string activityDate)
            {
                CircleName = circleName;
                CircleDescription = circleDescription;
                MemberCount = memberCount;
                BelongedSchool = belongedSchool;
                Notes = notes;
                WebAddress = webAddress;
                Contact = contact;
                CanInterColledge = canInterColledge;
                ActivityDate = activityDate;
            }

            public string CircleName { get; set; }

            public string CircleDescription { get; set; }

            public int MemberCount { get; set; }

            public string WebAddress { get; set; }

            public string BelongedSchool { get; set; }

            public string Notes { get; set; }

            public string Contact { get; set; }

            public bool CanInterColledge { get; set; }

            public string ActivityDate { get; set; }

            public static GetPutCircleDetailBody FromCircle(Circle circle)
            {
                return new GetPutCircleDetailBody(circle.Name, circle.Description, circle.MemberCount,
                    circle.BelongedSchool, circle.Notes, circle.WebAddress, circle.Contact, circle.CanInterColledge,
                    circle.ActivityDate);
            }
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
            public static GetCircleResponseCircleEntity FromCircle(Circle circle, bool isBelonging)
            {
                return new GetCircleResponseCircleEntity(circle.Name, circle.MemberCount, circle.BelongedSchool,
                    circle.LastModefied.ToString("yyyy年M月d日"), isBelonging, circle.Id.ToString());
            }

            public GetCircleResponseCircleEntity(string circleName, int memberCount, string belongedUniversity,
                string lastUpdateDate, bool isBelonged, string circleId)
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
}
