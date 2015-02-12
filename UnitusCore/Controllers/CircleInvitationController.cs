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
using UnitusCore.Results;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleInvitationController : UnitusApiController
    {

        [Route("CircleInvitation")]
        [Authorize]
        [EnableCors(GlobalConstants.CorsOrigins, "*", "*")]
        [HttpGet]
        public async Task<IHttpActionResult> PostInvitation(string address,string circleId)
        {
            return await this.OnValidToken("", () =>
            {
                //string[] inviteMembers = req.GetEmailList();
                Circle targetCircle = DbSession.Circles.Find(Guid.Parse(circleId));
                DbSession.Entry(CurrentUser).Collection(a=>a.AdministrationCircle).Load();
                CurrentUser.AdministrationCircle.Add(targetCircle);
                DbSession.SaveChanges();
                if (CurrentUser.AdministrationCircle.Contains(targetCircle))
                {
                    CircleInvitationManager.SendCircleInvitation(this, targetCircle,new string[] {address});
                }
                else
                {
                    return new BadRequestErrorMessageResult("権限が無効です",this);
                }
                return Json(true);
            });
        }

        public class CircleInvitationSendResponse
        {
            public bool HasWarning { get; set; }
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