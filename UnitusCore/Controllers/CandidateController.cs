using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UnitusCore.Attributes;
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CandidateController:UnitusApiController
    {
        [HttpGet]
        [ApiAuthorized]
        [UnitusCorsEnabled]
        [Route("Candidate/University")]
        public async Task<IHttpActionResult> Universities(string validationToken)
        {
            return await this.OnValidToken(validationToken,
                () =>
                {
                    HashSet<string> universities=new HashSet<string>();
                    var circleUniversities = DbSession.Circles.Select(a => a.BelongedSchool).Distinct();
                    var userUniversities = DbSession.People.Select(a => a.BelongedSchool).Distinct();
                    foreach (string university in circleUniversities)
                    {
                        if (string.IsNullOrWhiteSpace(university)) continue;
                        universities.Add(university);
                    }
                    foreach (string university in userUniversities)
                    {
                        if (string.IsNullOrWhiteSpace(university)) continue;
                        universities.Add(university);
                    }
                    return Json(ResultContainer<string[]>.GenerateSuccessResult(universities.ToArray()));
                });
        }
    }
}