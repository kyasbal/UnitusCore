using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Octokit.Helpers;
using UnitusCore.Attributes;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Achivement;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class AchivementController : UnitusApiController
    {
        public AchivementController()
        {
            
        }

        [HttpGet]
        [UnitusCorsEnabled]
        [Route("Achivements")]
        [Authorize]
        public async Task<IHttpActionResult> GetAchivementList(string validationToken)
        {
            return await this.OnValidToken(validationToken, async () =>
            {
                AchivementListResponse response=new AchivementListResponse();
                AchivementStatisticsStorage achivementStatistics=new AchivementStatisticsStorage(new TableStorageConnection());
                response.Achivements = (await achivementStatistics.EachForUserAchivements<AchivementListElement>(
                    CurrentUser.Id,
                    (a) =>
                    {
                        return new AchivementListElement(a.AchivementId,a.CurrentProgress,a.ProgressDiff,a.IsAwarded,a.IsAwarded?a.AwardedDate.FromUnixTime().ToString("d"):"");
                    }
                    )).ToArray();

                return Json(ResultContainer<AchivementListResponse>.GenerateSuccessResult(response));
            });
        }

        [HttpGet]
        [UnitusCorsEnabled]
        [Route("Achivement")]
        [Authorize]
        public async Task<IHttpActionResult> GetAchivement(string validationToken,string achivementName)
        {
            return await this.OnValidToken(validationToken, async () =>
            {
                AchivementStatisticsStorage achivementStatistics = new AchivementStatisticsStorage(new TableStorageConnection());
                var responseString=await achivementStatistics.GetCacheOrCalculate(achivementName, CurrentUser.Id,async () =>
                {
                    var response = await await achivementStatistics.RetrieveUserAchivement(CurrentUser.Id, achivementName, async (body, forUser, pg) =>
                    {
                        AchivementResponse result = new AchivementResponse();
                        result.AchivementName = body.AchivementName;
                        result.AchivementDescription = body.AchivementDescription;
                        result.CurrentProgress = forUser.CurrentProgress;
                        result.ProgressDiff = forUser.ProgressDiff;
                        result.IsAwarded = forUser.IsAwarded;
                        result.AwardedDate = forUser.AwardedDate.FromUnixTime().ToString("d");
                        result.AwardedRate = body.AwardedRate;
                        result.AwardedPerson = body.AwardedCount;
                        result.SumPerson = body.SumPeople;
                        result.ProgressGraphPoints = await pg.GenerateFromLastForUser(5, new TimeSpan(1, 0, 0, 0));
                        result.AcuireRateGraphPoints = await pg.GenerateFromLastForSystem(5, new TimeSpan(1, 0, 0, 0));
                        await CurrentUserWithPerson.PersonData.LoadBelongingCircles(DbSession);
                        List<AchivementResponseCircleElement> circleElements = new List<AchivementResponseCircleElement>();
                        foreach (MemberStatus stat in CurrentUser.PersonData.BelongedCircles)
                        {
                            await stat.LoadReferencesAsync(DbSession);
                            string circleId = stat.TargetCircle.Id.ToString();
                            List<AchivementResponseCircleMemberElement> members = new List<AchivementResponseCircleMemberElement>();
                            await stat.TargetCircle.LoadMembers(DbSession);
                            Dictionary<string, MemberStatus> membersById = new Dictionary<string, MemberStatus>();
                            foreach (MemberStatus circleMembers in stat.TargetCircle.Members)
                            {
                                await circleMembers.LoadReferencesAsync(DbSession);
                                await circleMembers.
                                    TargetUser.LoadApplicationUser(DbSession);
                                membersById.Add(circleMembers.TargetUser.ApplicationUser.Id, circleMembers);
                            }
                            var rankList = await achivementStatistics.GetRankingList(circleId, achivementName);
                            foreach (AchivementCircleRankingStatistics rankstatistics in rankList)
                            {
                                var stdata = membersById[rankstatistics.RowKey];
                                var statistics = await achivementStatistics.RetrieveAchivementProgressForUser(achivementName,
                                    rankstatistics.RowKey);
                                members.Add(new AchivementResponseCircleMemberElement()
                                {
                                    UserId = rankstatistics.RowKey,
                                    UserName = stdata.TargetUser.Name,
                                    AwardedDate = statistics.IsAwarded ? statistics.AwardedDate.FromUnixTime().ToString("d") : "",
                                    ProgressDiff = statistics.ProgressDiff,
                                    CurrentProgress = statistics.CurrentProgress,
                                    IsAwarded = statistics.IsAwarded,
                                    RankingInCircle = rankstatistics.Rank
                                });
                            }
                            circleElements.Add(new AchivementResponseCircleElement(stat.TargetCircle.Name, members.ToArray()));
                        }
                        result.CircleStatistics = circleElements.ToArray();
                        return result;
                    });
                    return
                        System.Web.Helpers.Json.Encode(
                            ResultContainer<AchivementResponse>.GenerateSuccessResult(response));
                });

                return Content(HttpStatusCode.OK, responseString,new RawJsonMediaTypeFormatter(),new MediaTypeWithQualityHeaderValue("application/json"));
            });
        }

        public class AchivementListResponse
        {
            public AchivementListElement[] Achivements { get; set; }
        }

        public class AchivementListElement
        {
            public AchivementListElement(string achivementName, double currentProgress, double progressDiff, bool isAwarded, string awardedDate)
            {
                AchivementName = achivementName;
                CurrentProgress = currentProgress;
                ProgressDiff = progressDiff;
                IsAwarded = isAwarded;
                AwardedDate = awardedDate;
            }

            public AchivementListElement()
            {
            }

            public string AchivementName { get; set; }

            public double CurrentProgress { get; set; }

            public double ProgressDiff { get; set; }

            public bool IsAwarded { get; set; }

            public string AwardedDate { get; set; }
        }

        public class AchivementResponse
        {
            public string AchivementName { get; set; }

            public string AchivementDescription { get; set; }

            public double CurrentProgress { get; set; }

            public double ProgressDiff { get; set; }

            public bool IsAwarded { get; set; }

            public string AwardedDate { get; set; }

            public double AwardedRate { get; set; }

            public int AwardedPerson { get; set; }

            public int SumPerson { get; set; }

            public string[][] ProgressGraphPoints { get; set; }

            public string[][] AcuireRateGraphPoints { get; set; }

            public AchivementResponseCircleElement[] CircleStatistics { get; set; }
        }

        public class AchivementResponseCircleElement
        {
            public string CircleName { get; set; }

            public AchivementResponseCircleElement(string circleName, AchivementResponseCircleMemberElement[] members)
            {
                CircleName = circleName;
                Members = members;
            }

            public AchivementResponseCircleMemberElement[] Members { get; set; }
        }

        public class AchivementResponseCircleMemberElement
        {
            public string UserId { get; set; }

            public string UserName { get; set; }

            public double CurrentProgress { get; set; }

            public double ProgressDiff { get; set; }

            public int RankingInCircle { get; set; }

            public bool IsAwarded { get; set; }

            public string AwardedDate { get; set; }

        }
    }
}
