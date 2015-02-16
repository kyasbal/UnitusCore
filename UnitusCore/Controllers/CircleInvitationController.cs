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
        [ApiAuthorized]
        [UnitusCorsEnabled]
        [HttpPost]
        public async Task<IHttpActionResult> PostInvitation(CircleInvitationSendRequest req)
        {
            return await this.OnValidToken("", () =>
            {
                Circle targetCircle = DbSession.Circles.Find(Guid.Parse(req.CircleId));
                DbSession.Entry(CurrentUser).Collection(a=>a.AdministrationCircle).Load();
                CurrentUser.AdministrationCircle.Add(targetCircle);
                DbSession.SaveChanges();
                if (CurrentUser.AdministrationCircle.Contains(targetCircle))
                {
                    var result=CircleInvitationManager.SendCircleInvitation(this, targetCircle,req.GetEmailList());
                    return Json(ResultContainer<CircleInvitationResult>.GenerateSuccessResult(result));
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

            public CircleInvitationSendRequest(string circleId, string address)
            {
                CircleId = circleId;
                Address = address;
            }

            public string CircleId { get; set; }

            public string Address { get; set; }

            public string[] GetEmailList()
            {
                return Address.Split('\n');
            }
        }
    }
}