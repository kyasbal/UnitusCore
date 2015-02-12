using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using UnitusCore.Attributes;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleInvitation : UnitusApiController
    {

        [Route("Circle/Invite")]
        [Authorize]
        [EnableCors(GlobalConstants.CorsOrigins, "*", "*")]
        public async Task<IHttpActionResult> PostInvitation(CircleInvitationSendRequest req)
        {
            return await this.OnValidToken(req, (r) =>
            {
                string[] inviteMembers = req.GetEmailList();
                Circle targetCircle = DbSession.Circles.Find(r.CircleId);
                if (CurrentUser.AdministrationCircle.Contains(targetCircle))
                {

                }
                else
                {
                    return new BadRequestErrorMessageResult("権限が無効です",this);
                }
                return Json(true);
            });
        }

        public class CircleInvitationSendRequest : AjaxRequestModelBase
        {
            public CircleInvitationSendRequest()
            {

            }

            public CircleInvitationSendRequest(string circleId, string sendEmails)
            {
                CircleId = circleId;
                SendEmails = sendEmails;
            }

            public string CircleId { get; set; }

            public string SendEmails { get; set; }

            public string[] GetEmailList()
            {
                return SendEmails.Split('\n');
            }
        }
    }
}