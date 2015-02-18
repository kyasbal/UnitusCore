using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.Base
{
    public class TableStorageConnection:AzureStorageConnection
    {
        public readonly CloudTableClient TableClient;

        public TableStorageConnection()
        {
            TableClient = StorageAccount.CreateCloudTableClient();
        }
    }
}