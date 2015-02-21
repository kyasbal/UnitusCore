using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;

namespace UnitusCore.Storage.DataModels
{
    public class CircleMemberCache:TableEntity
    {
        public CircleMemberCache()
        {
            
        }

        public CircleMemberCache(string circleId, string userId, string personId)
        {
            PartitionKey = circleId;
            RowKey=UserId = userId;
            PersonId = personId;
        }

        public string UserId { get; set; }

        public string PersonId { get; set; }
    }
}