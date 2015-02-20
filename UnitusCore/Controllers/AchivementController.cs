using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Octokit.Helpers;
using UnitusCore.Attributes;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
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
                var response=await await achivementStatistics.RetrieveUserAchivement(CurrentUser.Id,achivementName,async (body,forUser,pg)=>
                {
                    AchivementResponse result=new AchivementResponse();
                    result.AchivementName = body.AchivementName;
                    result.AchivementDescription = body.AchivementDescription;
                    result.CurrentProgress = forUser.CurrentProgress;
                    result.ProgressDiff = forUser.ProgressDiff;
                    result.IsAwarded = forUser.IsAwarded;
                    result.AwardedDate = forUser.AwardedDate.FromUnixTime().ToString("d");
                    result.AwardedRate = body.AwardedRate;
                    result.AwardedPerson = body.AwardedCount;
                    result.SumPerson = body.SumPeople;
                    result.ProgressGraphPoints =await pg.GenerateFromLastForUser(5, new TimeSpan(1, 0, 0, 0));
                    result.AcuireRateGraphPoints = await pg.GenerateFromLastForSystem(5, new TimeSpan(1, 0, 0, 0));
                    return result;
                });
                return Json(ResultContainer<AchivementResponse>.GenerateSuccessResult(response));
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
        }
    }
}
