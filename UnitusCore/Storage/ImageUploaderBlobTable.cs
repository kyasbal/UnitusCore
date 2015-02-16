using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Blob;
using UnitusCore.Storage.Base;

namespace UnitusCore.Storage
{
    [BlobStorage("image-uploader")]
    public class ImageUploaderBlobTable:BlobStorageTableBase
    {
        public ImageUploaderBlobTable(BlobStorageConnection connection) : base(connection)
        {

        }

        public async Task AddImage(string imageId,Stream stream,string mime)
        {
            CloudBlockBlob blobRef = Container.GetBlockBlobReference(imageId);
            if (blobRef.Exists())
            {
                throw new InvalidDataException("duplicated key");
            }
            else
            {
                blobRef.Properties.ContentType = mime;
                await blobRef.UploadFromStreamAsync(stream);
            }
        }

        public async Task<ObtainImageUploaderBlobResponse> DownloadImage(string imageId)
        {
            CloudBlockBlob blobRef = Container.GetBlockBlobReference(imageId);
            
            if (!blobRef.Exists())
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            else
            {
                MemoryStream ms=new MemoryStream();
                await blobRef.DownloadToStreamAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return new ObtainImageUploaderBlobResponse(ms, blobRef.Properties.ContentType);
            }
        }


    }

    public class ObtainImageUploaderBlobResponse
    {
        public Stream ContentStream { get; private set; }

        public string MimeType { get; private set; }

        public ObtainImageUploaderBlobResponse(Stream contentStream, string mimeType)
        {
            ContentStream = contentStream;
            MimeType = mimeType;
        }
    }

}