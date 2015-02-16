using System.IO;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Blob;

namespace UnitusCore.Storage.Base
{
    public class BlobStorageTableBase
    {
        protected readonly BlobStorageConnection Connection;

        public BlobStorageTableBase(BlobStorageConnection connection)
        {
            Connection = connection;
            var storageAttribute = GetType().GetCustomAttribute<BlobStorageAttribute>();
            if (storageAttribute != null)
            {
                Container = connection.BlobClient.GetContainerReference(storageAttribute.ContainerName);
                Container.CreateIfNotExists();
            }
            else
            {
                throw  new InvalidDataException("このクラスにはテーブル名を指定するAttributeを設置する必要があります。");
            }
        }

        protected readonly CloudBlobContainer Container;
    }
}