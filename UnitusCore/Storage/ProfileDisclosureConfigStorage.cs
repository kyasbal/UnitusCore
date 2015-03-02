using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Profile;

namespace UnitusCore.Storage
{
    public class ProfileDisclosureConfigStorage:TableStorageBase
    {
        private readonly string _userId;
        private const string ProfileDisclosureConfigTableName = "ProfileDisclosureConfig";

        private readonly CloudTable _profileDisclosureConfigTable;

        public ProfileDisclosureConfigStorage(TableStorageConnection storageConnection,string userId) : base(storageConnection)
        {
            _userId = userId;
            _profileDisclosureConfigTable = InitCloudTable(ProfileDisclosureConfigTableName);
        }

        private async Task<DisclosureConfig> RetrieveDisclosureConfigEntity(string userId,ProfileProperty property)
        {
            return
                (DisclosureConfig) (await
                    _profileDisclosureConfigTable.ExecuteAsync(TableOperation.Retrieve<DisclosureConfig>(userId,
                        property.ToString()))).Result;
        }

        private async Task<DisclosureConfig> RetrieveDefaultConfigEntity(string userId,ProfileProperty property)
        {
            var defaultData=await RetrieveDisclosureConfigEntity("DEFAULT", property);
            if (defaultData == null) return null;
            return new DisclosureConfig(userId, property, defaultData.Config);
        }

        private DisclosureConfig FromDefaultConfig(string userId,DisclosureConfig defaultDisclosureConfig)
        {
            return new DisclosureConfig(userId,(ProfileProperty) Enum.Parse(typeof(ProfileProperty),defaultDisclosureConfig.RowKey),defaultDisclosureConfig.Config);
        }

        private async Task<ProfilePropertyDisclosureConfig> FetchDisclosureConfig(string userId,ProfileProperty property)
        {
            var disclosureConfigEntity =await RetrieveDisclosureConfigEntity(userId, property);
            if (disclosureConfigEntity == null)
            {
                disclosureConfigEntity = await RetrieveDefaultConfigEntity(userId,property);
                if (disclosureConfigEntity == null)
                {
                    var defaultEntity=new DisclosureConfig("DEFAULT",property,ProfilePropertyDisclosureConfig.Public);
                    await AddDisclosureConfig(defaultEntity);
                    disclosureConfigEntity = FromDefaultConfig(userId, defaultEntity);
                }
                await AddDisclosureConfig(disclosureConfigEntity);
            }
            return disclosureConfigEntity.Config;
        }

        private bool HasRight(AccessBy accessBy, ProfilePropertyDisclosureConfig disclosureConfig)
        {
            return (((byte)accessBy)&((byte)disclosureConfig))!=0x00;
        }

        private async Task AddDisclosureConfig(string userId,ProfileProperty property,ProfilePropertyDisclosureConfig disclosureConfig)
        {
            await AddDisclosureConfig(new DisclosureConfig(userId, property, disclosureConfig));
        }

        private async Task AddDisclosureConfig(DisclosureConfig config)
        {
            await
                _profileDisclosureConfigTable.ExecuteAsync(
                    TableOperation.InsertOrReplace(config));
        }

        public async Task<DisclosureProtectedResponse> FetchProtectedProperty(ProfileProperty property,AccessBy accessBy,Func<Task<string>> fetchContent)
        {
            ProfilePropertyDisclosureConfig disclosureConfig = await FetchDisclosureConfig(_userId, property);
            if (HasRight(accessBy, disclosureConfig))
            {
                var content = await fetchContent();
                return new DisclosureProtectedResponse(disclosureConfig, content, true);
            }
            else
            {
                return new DisclosureProtectedResponse(disclosureConfig, null, false);
            }
        }

        public async Task<DisclosureProtectedResponse> FetchProtectedProperty(ProfileProperty property, AccessBy accessBy, Func<string> fetchContent)
        {
            return await FetchProtectedProperty(property,accessBy, () => Task.Run(fetchContent));
        }
    }

    [Flags]
    public enum AccessBy
    {
        AnonymousCircle=0x04,
        Circle=0x06,
        Owner=0x07
    }

    public class DisclosureProtectedResponse
    {
        public DisclosureProtectedResponse()
        {
            
        }

        public DisclosureProtectedResponse(ProfilePropertyDisclosureConfig disclosureConfig, string content, bool canAccess)
        {
            DisclosureConfig = disclosureConfig;
            Content = content;
            CanAccess = canAccess;
        }

        public ProfilePropertyDisclosureConfig DisclosureConfig { get; set; }

        public string Content { get; set; }

        public bool CanAccess { get; set; }
    }
}