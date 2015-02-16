using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using UnitusCore.Attributes;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class UploaderController : ApiController
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

        [HttpPost]
        [Route("Uploader/Upload")]
        [UnitusCorsEnabled]
        public async Task<IHttpActionResult> UploadImage()
        {
            HttpRequestMessage request = this.Request;
            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }
            else
            {
                List<string> targetAddrs=new List<string>();
                var multiPartData = await request.Content.ReadAsMultipartAsync();
                foreach (HttpContent content in multiPartData.Contents)
                {
                    string id = IdGenerator.GetId(16);
                    var contentType = content.Headers.ContentType;
                    if(contentType==null)continue;
                    await
                        BlobTable.AddImage(id, await content.ReadAsStreamAsync(),contentType.MediaType);
                    targetAddrs.Add(Url.Content(Url.Route("UploaderDownload",new Dictionary<string, object>(){{"imageId",id}})));
                }
                return Json(ResultContainer<string[]>.GenerateSuccessResult(targetAddrs.ToArray()));
            }
        }

        [HttpGet]
        [Route(template:"Uploader/Download",Name = "UploaderDownload")]
        [UnitusCorsEnabled]
        public async Task<IHttpActionResult> DownloadImage(string imageId)
        {
            var data = await BlobTable.DownloadImage(imageId);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StreamContent(data.ContentStream);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(data.MimeType);
            return new ResponseMessageResult(response);
        }


    }
}
