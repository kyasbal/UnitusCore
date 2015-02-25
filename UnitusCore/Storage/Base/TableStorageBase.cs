using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.Base
{
    public class TableStorageBase
    {
        protected TableStorageConnection _storageConnection;

        public TableStorageBase(TableStorageConnection storageConnection)
        {
            _storageConnection = storageConnection;
        }

        protected CloudTable InitCloudTable(string referenceName)
        {
            var table = _storageConnection.TableClient.GetTableReference(referenceName);
            table.CreateIfNotExists();
            return table;
        }

    }
}