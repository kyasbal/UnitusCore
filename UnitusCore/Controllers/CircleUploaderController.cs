using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Octokit;
using UnitusCore.Attributes;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class CircleUploaderController:UnitusApiController
    {
        private ImageUploaderBlobTable _blobTable;

        private ImageUploaderBlobTable BlobTable
        {
            get
            {
                _blobTable = _blobTable ?? GetBlobTable();
                return _blobTable;
            }
        }

        private ImageUploaderBlobTable GetBlobTable()
        {
            BlobStorageConnection connection = new BlobStorageConnection();
            return new ImageUploaderBlobTable(connection);
        }

        private string GetFirstHeaderElement(string name,HttpRequestHeaders header)
        {
            IEnumerable<string> result = null;
            if(!header.TryGetValues(name,out result))throw new HttpResponseException(HttpStatusCode.BadRequest);
            string value = result.FirstOrDefault();
            if(string.IsNullOrWhiteSpace(value))throw new HttpResponseException(HttpStatusCode.BadRequest);
            return value;
        }

        private async Task<Circle> GetCircleFromId(string id)
        {
            Guid circleGuid;
            if(!Guid.TryParse(id,out circleGuid))throw new HttpResponseException(HttpStatusCode.BadRequest);
            var circle=await DbSession.Circles.FindAsync(circleGuid);
            if(circle==null)throw new HttpResponseException(HttpStatusCode.NotFound);
            return circle;
        }

        private async Task<bool> IsCircleMember(Circle circle)
        {
            var belongedToStatus = DbSession.Entry(CurrentUserWithPerson.PersonData).Collection(a => a.BelongedCircles);
            if (!belongedToStatus.IsLoaded) await belongedToStatus.LoadAsync();
            foreach (MemberStatus belongingStatus in belongedToStatus.CurrentValue)
            {
                var circleStatus = DbSession.Entry(belongingStatus).Reference(a => a.TargetCircle);
                if (!circleStatus.IsLoaded) await circleStatus.LoadAsync();
                if (circleStatus.CurrentValue.Id.Equals(circle.Id)) return true;
            }
            return false;
        }

        
        [UnitusCorsEnabled]
        [ApiAuthorized]
        [HttpPost]
        [Route("Uploader/UploadForCircle")]
        public async Task<IHttpActionResult> Upload()
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            else
            {
                //リクエストヘッダーから読み込む
                UploadRequest requestArgument=new UploadRequest();
                requestArgument.CircleId = GetFirstHeaderElement("CircleId", request.Headers);
                requestArgument.ValidationToken = GetFirstHeaderElement("ValidationToken", request.Headers);
                Circle circle = await GetCircleFromId(requestArgument.CircleId);
                if(!await IsCircleMember(circle))throw new HttpResponseException(HttpStatusCode.Unauthorized);
                return await this.OnValidToken(requestArgument.ValidationToken,async () =>
                {
                    List<string> targetAddrs = new List<string>();
                    var multiPartData = await request.Content.ReadAsMultipartAsync();
                    foreach (HttpContent content in multiPartData.Contents)
                    {
                        string id = IdGenerator.GetId(16);
                        var contentType = content.Headers.ContentType;
                        if (contentType == null) continue;
                        await
                            BlobTable.AddImage(id, await content.ReadAsStreamAsync(), contentType.MediaType);
                        string urlAddr =
                            Url.Content(Url.Route("UploaderDownload", new Dictionary<string, object>() {{"imageId", id}}));
                        targetAddrs.Add(urlAddr);
                        CircleUploaderEntity entity=new CircleUploaderEntity();
                        entity.GenerateId();
                        entity.UploadedCircle = circle;
                        entity.UploadUser = CurrentUser;
                        entity.TargetAddress = urlAddr;
                        DbSession.CircleUploaderEntities.Add(entity);
                    }
                    await DbSession.SaveChangesAsync();
                    return Json(ResultContainer<string[]>.GenerateSuccessResult(targetAddrs.ToArray()));
                });
            }
        }

        [UnitusCorsEnabled]
        [ApiAuthorized]
        [HttpGet]
        [Route("Uploader/ListForCircle")]
        public async Task<IHttpActionResult> GetUploaderListForCircle(string validationToken,string circleId)
        {
            return await this.OnValidToken(validationToken, async () =>
            {
                Circle circle =await GetCircleFromId(circleId);
                var uploaderEntitiesStatus = DbSession.Entry(circle).Collection(a => a.UploadedEntities);
                if (!uploaderEntitiesStatus.IsLoaded) await uploaderEntitiesStatus.LoadAsync();
                List<UploaderListEntity> entities=new List<UploaderListEntity>();
                foreach (CircleUploaderEntity entity in circle.UploadedEntities)
                {
                    var entityTargetUserStatus = DbSession.Entry(entity).Reference(a => a.UploadUser);
                    if (!entityTargetUserStatus.IsLoaded) await entityTargetUserStatus.LoadAsync();
                    var personDataStatus = DbSession.Entry(entity.UploadUser).Reference(a => a.PersonData);
                    if (!personDataStatus.IsLoaded) await personDataStatus.LoadAsync();
                    entities.Add(new UploaderListEntity(personDataStatus.CurrentValue.Name,entity.LastModefied.ToString("yyyy年mmmm月dd日"),entity.TargetAddress));
                }
                return Json(ResultContainer<UploaderListEntity[]>.GenerateSuccessResult(entities.ToArray()));
            });
        }

        public class UploadRequest:AjaxRequestModelBase
        {
             public string CircleId { get; set; }
        }

        public class UploaderListEntity
        {
            public UploaderListEntity(string userName, string date, string linkAddr)
            {
                UserName = userName;
                Date = date;
                LinkAddr = linkAddr;
            }

            public string UserName { get; set; }

            public string Date { get; set; }

            public string LinkAddr { get; set; }
        }
    }


}