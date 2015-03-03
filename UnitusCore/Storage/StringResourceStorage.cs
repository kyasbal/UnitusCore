using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels;

namespace UnitusCore.Storage
{
    public class StringResourceStorage:TableStorageBase
    {
        private static StringResourceStorage _instance;

        public static StringResourceStorage Instance
        {
            get
            {
                _instance = _instance ?? new StringResourceStorage(new TableStorageConnection());
                return _instance;
            }
        }

        private readonly string StringResourceTableName = "StringResources";

        private readonly CloudTable StringResourceTable;

        private readonly CultureInfo targetCultureInfo;

        public StringResourceStorage(TableStorageConnection storageConnection) : base(storageConnection)
        {
            StringResourceTable = InitCloudTable(StringResourceTableName);
            targetCultureInfo=new CultureInfo("JA-JP");
        }

        private async Task<StringResource> RetrieveStringResource(string propName)
        {
            return
                (StringResource) (await
                    StringResourceTable.ExecuteAsync(
                        TableOperation.Retrieve<StringResource>(targetCultureInfo.NativeName, propName))).Result;
        }

        private async Task<StringResource> AppendDefault(string propName)
        {
            StringResource resource = new StringResource(propName, "DEFAULT VALUE");
            await StringResourceTable.ExecuteAsync(TableOperation.InsertOrReplace(resource));
            return resource;
        }

        public async Task<string> GetPropertyString(string propName)
        {
            var retrievedBody = await RetrieveStringResource(propName);
            if (retrievedBody == null)
            {
                retrievedBody=await AppendDefault(propName);
            }
            return retrievedBody.Body;
        }
    }
}