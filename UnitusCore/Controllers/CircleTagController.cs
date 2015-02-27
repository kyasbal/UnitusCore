using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UnitusCore.Attributes;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleTagController:UnitusApiController
    {
        [Authorize]
        [HttpPost]
        [Route("Circle/Tag")]
        [CircleAdmin("circleId")]
        public async Task<IHttpActionResult> AddNewTag(AddNewTagRequest req)
        {
            return await this.OnValidToken(req, async (r) =>
            {
                CircleTagStorage storage = new CircleTagStorage(new TableStorageConnection());
                await storage.GenerateNewTag(req.CircleId, req.TagName, req.IsVisibleForNonAdmin);
                return Json(true);
            });
        }

        [Authorize]
        [HttpPut]
        [Route("Circle/Tag")]
        [CircleAdmin("circleId")]
        public async Task<IHttpActionResult> ApplyTag(ApplyTagRequest req)
        {
            return await this.OnValidToken(req, async (r) =>
            {
                //userIdの存在チェック
                ApplicationUser user = DbSession.Users.Find(req.UserId);
                if(user==null)throw new HttpResponseException(HttpStatusCode.NotFound);
                CircleTagStorage storage=new CircleTagStorage(new TableStorageConnection());
                if((await storage.RetrieveTagBody(req.CircleId,req.TagName))==null)throw new HttpResponseException(HttpStatusCode.NotFound);
                await storage.ApplyTag(req.CircleId, req.UserId, req.TagName);
                return Json(true);
            });
        }

        [Authorize]
        [HttpDelete]
        [Route("Circle/Tag")]
        [CircleAdmin("circleId")]
        public async Task<IHttpActionResult> RemoveTag(ApplyTagRequest req)
        {
            return await this.OnValidToken(req, async (r) =>
            {
                //userIdの存在チェック
                ApplicationUser user = DbSession.Users.Find(req.UserId);
                if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
                CircleTagStorage storage = new CircleTagStorage(new TableStorageConnection());
                if ((await storage.RetrieveTagBody(req.CircleId, req.TagName)) == null) throw new HttpResponseException(HttpStatusCode.NotFound);
                await storage.DeleteTag(req.CircleId, req.UserId, req.TagName);
                return Json(true);
            });
        }

        public class ApplyTagRequest : AjaxRequestModelBase
        {
            public string CircleId { get; set; }

            public string TagName { get; set; }

            public string UserId { get; set; }
        }

        public class AddNewTagRequest:AjaxRequestModelBase
        {
            public string CircleId { get; set; }

            public string TagName { get; set; }

            public bool IsVisibleForNonAdmin { get; set; }
        }
    }
}