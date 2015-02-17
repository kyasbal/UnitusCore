using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using UnitusCore.Attributes;
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class AccountApiController:UnitusApiController
    {
        [Route("Config/Password")]
        [HttpPut]
        [UnitusCorsEnabled]
        [ApiAuthorized]
        [HttpsApi]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordRequest req)
        {
            return await this.OnValidToken(req, async (r) =>
            {
                var identityResult = await UserManager.ChangePasswordAsync(CurrentUser.Id, req.CurrentPassword, req.NewPassword);
                if (identityResult.Succeeded)
                {
                    return Json(ResultContainer.GenerateSuccessResult());
                }
                else
                {
                    return Json(ResultContainer<IdentityResult>.GenerateSuccessResult(identityResult));
                }
            });
        }

        public class ChangePasswordRequest : AjaxRequestModelBase
        {
            public string CurrentPassword { get; set; }

            public string NewPassword { get; set; }
        }
    }
}