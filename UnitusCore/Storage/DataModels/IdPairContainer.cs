using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels
{
    public class IdPairContainer:TableEntity
    {
        public const string GithubLogin = "GithubLogin";

        public const string UserId = "UserId";

        public const string PersonId = "PersonId";

        public IdPairContainer()
        {
            
        }

        public IdPairContainer(string partitionKey,string rowKey,string id)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
            this.TargetId = id;
        }

        public string TargetId { get; set; }

    }
}