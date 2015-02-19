using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Queue;

namespace UnitusCore.Storage.Base
{
    public class QueueStorageConnection:AzureStorageConnection
    {
        public readonly CloudQueueClient Client;

        public QueueStorageConnection()
        {
            Client = StorageAccount.CreateCloudQueueClient();
        }
    }
}