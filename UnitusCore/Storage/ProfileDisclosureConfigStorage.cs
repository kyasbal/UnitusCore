using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using UnitusCore.Controllers;
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

        private async Task<DisclosureConfig> FetchDisclosureConfig(string userId,ProfileProperty property)
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
            return disclosureConfigEntity;
        }

        private async Task<ProfilePropertyDisclosureConfig> FetchDisclosureConfigFlag(string userId,
            ProfileProperty property)
        {
            return (await FetchDisclosureConfig(userId, property)).Config;
        }

        private bool HasRight(AccessBy accessBy, ProfilePropertyDisclosureConfig disclosureConfig)
        {
            return (((byte)accessBy)&((byte)disclosureConfig))!=0x00;
        }

        public async Task AddDisclosureConfig(string userId,ProfileProperty property,ProfilePropertyDisclosureConfig disclosureConfig)
        {
            await AddDisclosureConfig(new DisclosureConfig(userId, property, disclosureConfig));
        }

        private async Task AddDisclosureConfig(DisclosureConfig config)
        {
            await
                _profileDisclosureConfigTable.ExecuteAsync(
                    TableOperation.InsertOrReplace(config));
        }

        public async Task<DisclosureProtectedResponse<T>> FetchProtectedProperty<T>(ProfileProperty property,AccessBy accessBy,Func<Task<T>> fetchContent)where T:class
        {
            ProfilePropertyDisclosureConfig disclosureConfig = await FetchDisclosureConfigFlag(_userId, property);
            if (HasRight(accessBy, disclosureConfig))
            {
                var content = await fetchContent();
                return new DisclosureProtectedResponse<T>(disclosureConfig, content, true);
            }
            else
            {
                return new DisclosureProtectedResponse<T>(disclosureConfig, null, false);
            }
        }

        public async Task<DisclosureProtectedResponse<T>> FetchProtectedProperty<T>(ProfileProperty property, AccessBy accessBy, Func<T> fetchContent) where T : class
        {
            return await FetchProtectedProperty(property,accessBy, () => Task.Run(fetchContent));
        }

        public async Task<IEnumerable<DisclosureConfig>> GetAllDisclosureProtectedConfigure()
        {
            HashSet<DisclosureConfig> configs=new HashSet<DisclosureConfig>();
            foreach (ProfileProperty targetProperty in Enum.GetValues(typeof (ProfileProperty)))
            {
                configs.Add(await FetchDisclosureConfig(_userId,targetProperty));
            }
            return configs;
        }

        public async Task ObtainDetailedNames(IEnumerable<IDetailedDisplayDisclosureConfig> configures)
        {
            foreach (IDetailedDisplayDisclosureConfig config in configures)
            {
                config.DisplayConfigureName=await StringResourceStorage.Instance.GetPropertyString("ConfigDetailedName-" + config.Property);
            }
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
        public DisclosureProtectedResponse(ProfilePropertyDisclosureConfig disclosureConfig,bool canAccess)
        {
            DisclosureConfig = disclosureConfig;
            CanAccess = canAccess;
        }

        public ProfilePropertyDisclosureConfig DisclosureConfig { get; set; }
        public bool CanAccess { get; set; }
    }

    public class DisclosureProtectedResponse<T> : DisclosureProtectedResponse
    {
        public DisclosureProtectedResponse()
        {
            
        }

        public DisclosureProtectedResponse(ProfilePropertyDisclosureConfig disclosureConfig, T content, bool canAccess) : base(disclosureConfig,canAccess)
        {
            Content = content;
        }

        public T Content { get; set; }

        
    }
}