using Microsoft.WindowsAzure.Storage.Blob;

namespace UnitusCore.Storage.Base
{
    public class BlobStorageConnection : AzureStorageConnection
    {
        public BlobStorageConnection()
        {
            BlobClient = StorageAccount.CreateCloudBlobClient();
        }

        public CloudBlobClient BlobClient { get; set; }
    }
}