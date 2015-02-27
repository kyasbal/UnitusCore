using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Circle;
using UnitusCore.Util;

namespace UnitusCore.Storage
{
    public class CircleTagStorage:TableStorageBase
    {
        private const string CircleMemberTagBodyTableName = "CircleMemberTagBody";

        private const string CircleMemberTagGlueTableName = "CircleMemberTagGlue";

        private readonly CloudTable _circleMemberTagBodyTable;

        private readonly CloudTable _circleMemberTagGlueTable;

        public CircleTagStorage(TableStorageConnection storageConnection) : base(storageConnection)
        {
            _circleMemberTagBodyTable = InitCloudTable(CircleMemberTagBodyTableName);
            _circleMemberTagGlueTable = InitCloudTable(CircleMemberTagGlueTableName);
        }

        public async Task<IEnumerable<CircleMemberTagBody>> RetrieveTagBodies(string circleId,bool includeNonVisibleForNonAdmin=false)
        {
            TableQuery<CircleMemberTagBody> tagQuery=new TableQuery<CircleMemberTagBody>().Where(TableQuery.GenerateFilterCondition("PartitionKey",QueryComparisons.Equal,circleId));
            if (includeNonVisibleForNonAdmin)
            {
                return _circleMemberTagBodyTable.ExecuteQuery(tagQuery);
            }
            else
            {
                return _circleMemberTagBodyTable.ExecuteQuery(tagQuery).Where(a => a.IsVisibleForNonAdmin);
            }
        }

        public async Task<CircleMemberTagBody> RetrieveTagBody(string circleId,string tagName)
        {
            return
                (CircleMemberTagBody) (await
                    _circleMemberTagBodyTable.ExecuteAsync(TableOperation.Retrieve<CircleMemberTagBody>(circleId,
                        tagName.ToHashCode()))).Result;
        }

        public async Task GenerateNewTag(string circleId, string tag, bool isVisibleForNonAdmin)
        {
           CircleMemberTagBody tagBody=new CircleMemberTagBody(circleId,tag,isVisibleForNonAdmin);
            await _circleMemberTagBodyTable.ExecuteAsync(TableOperation.InsertOrReplace(tagBody));
        }

        public async Task ApplyTag(string circleId, string userId, string tag)
        {
            CircleMemberTagGlue fowardGlue=new CircleMemberTagGlue();
            fowardGlue.PartitionKey = CircleMemberTagGlue.GeneratePartitionKey(circleId, userId);
            fowardGlue.GluedValue = tag;
            CircleMemberTagGlue backGlue = new CircleMemberTagGlue();
            backGlue.PartitionKey = CircleMemberTagGlue.GeneratePartitionKey(circleId, tag);
            backGlue.GluedValue = userId;
            await _circleMemberTagGlueTable.ExecuteAsync(TableOperation.InsertOrReplace(fowardGlue));
            await _circleMemberTagGlueTable.ExecuteAsync(TableOperation.InsertOrReplace(backGlue));
        }

        private async Task<CircleMemberTagGlue> RetrieveGlue(string circleId, string glue, string value)
        {
            CircleMemberTagGlue glueRetrieveQuery =(CircleMemberTagGlue) (await 
                _circleMemberTagGlueTable.ExecuteAsync(
                    TableOperation.Retrieve<CircleMemberTagGlue>(
                        CircleMemberTagGlue.GeneratePartitionKey(circleId, glue), value.ToHashCode()))).Result;
            return glueRetrieveQuery;
        }

        public async Task DeleteTag(string circleId, string userId, string tag)
        {
            CircleMemberTagGlue fowardGlue = await RetrieveGlue(circleId, userId, tag);
            CircleMemberTagGlue backGlue = await RetrieveGlue(circleId, tag, userId);
            await _circleMemberTagGlueTable.ExecuteAsync(TableOperation.Delete(fowardGlue));
            await _circleMemberTagGlueTable.ExecuteAsync(TableOperation.Delete(backGlue));
        }

        public async Task<IEnumerable<string>> GetAppliedTags(string circleId,string userId,bool includeNonVisibleForNonAdmin=false)
        {
            TableQuery<CircleMemberTagGlue> query =
                new TableQuery<CircleMemberTagGlue>().Where(TableQuery.GenerateFilterCondition("PartitionKey",
                    QueryComparisons.Equal, CircleMemberTagGlue.GeneratePartitionKey(circleId, userId)));
            if (includeNonVisibleForNonAdmin)
            {
                return _circleMemberTagGlueTable.ExecuteQuery(query).Select(a => a.GluedValue);
            }
            else
            {
                var glues=_circleMemberTagGlueTable.ExecuteQuery(query).Select(a => a.RowKey);
                HashSet<string> result=new HashSet<string>();
                foreach (string glue in glues)
                {
                    if((await RetrieveTagBody(circleId, glue)).IsVisibleForNonAdmin)
                    {
                        result.Add(glue);
                    }
                }
                return result;
            }
        }

        public async Task<IEnumerable<string>> GetAppliedMembers(string circleId, string tagName)
        {
            TableQuery<CircleMemberTagGlue> query = new TableQuery<CircleMemberTagGlue>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, CircleMemberTagGlue.GeneratePartitionKey(circleId, tagName)));
            return _circleMemberTagGlueTable.ExecuteQuery(query).Select(a => a.GluedValue);
        }
    }
}