using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using UnitusCore.Attributes;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class UserConfigureController :UnitusApiController
    {
        [Route("Config")]
        [HttpGet]
        [UnitusCorsEnabled]
        [ApiAuthorized]
        public async Task<IHttpActionResult> GetUserConfigure(PutUserConfigureRequest req)
        {
            return await this.OnValidToken(req, async (r) =>
            {
                //ApplicationUser user = CurrentUserWithPerson;
                //await user.PersonData.LoadUserConfigure(DbSession);
                //user.PersonData.UserConfigure.ShowOwnProfileToOtherCircle = req.ShowOwnProfileToOtherCircle;
                //await DbSession.SaveChangesAsync();
                return Json(ResultContainer.GenerateSuccessResult());
            });
        }

        public class PutUserConfigureRequest:AjaxRequestModelBase
        {
            public bool ShowOwnProfileToOtherCircle { get; set; }
        }
    }
}
