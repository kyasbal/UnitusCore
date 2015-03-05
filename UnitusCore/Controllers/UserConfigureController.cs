using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnitusCore.Attributes;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Profile;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class UserConfigureController :UnitusApiControllerWithTableConnection
    {
        [Route("Config")]
        [HttpGet]
        [UnitusCorsEnabled]
        [ApiAuthorized]
        public async Task<IHttpActionResult> GetUserConfigure(string validationToken)
        {
            return await this.OnValidToken(validationToken, async () =>
            {

                return Json(ResultContainer<GetUserConfigureResponse>.GenerateSuccessResult(await GenerateGetUserConfigureResponse(CurrentUser.Id)),
                    new JsonSerializerSettings()
                    {
                    });
            });
        }

        [Route("Config/Disclosure")]
        [HttpPut]
        [UnitusCorsEnabled]
        [ApiAuthorized]
        public async Task<IHttpActionResult> PutUserDisclosureConfigure(DisplayDisclosureConfigRequest disclosureConfigRequest)
        {
            return await this.OnValidToken(disclosureConfigRequest, async (r) =>
            {
                await PutDisclosureConfig(CurrentUser.Id, disclosureConfigRequest);
                return Json(ResultContainer<GetUserConfigureResponse>.GenerateSuccessResult(await GenerateGetUserConfigureResponse(CurrentUser.Id)),
                    new JsonSerializerSettings()
                    {
                    });
            });
        }

        private async Task<GetUserConfigureResponse> GenerateGetUserConfigureResponse(string userId)
        {
            ProfileDisclosureConfigStorage storage=new ProfileDisclosureConfigStorage(TableConnection,userId);
            var disclosureConfigures =
                (await storage.GetAllDisclosureProtectedConfigure()).Select(
                    Mapper.DynamicMap<IDisplayDisclosureConfig, DisplayDisclosureConfig>).ToArray();
            await storage.ObtainDetailedNames(disclosureConfigures);
            return new GetUserConfigureResponse(){DisclosureConfigs = disclosureConfigures};
        }

        private async Task PutDisclosureConfig(string userId,IDisplayDisclosureConfig config)
        {
            ProfileDisclosureConfigStorage storage = new ProfileDisclosureConfigStorage(TableConnection, userId);
            await storage.AddDisclosureConfig(userId, Ensure.IsEnumElement<ProfileProperty>(config.Property),Ensure.IsEnumElement<ProfilePropertyDisclosureConfig>(config.ConfigString));
        }

        public class GetUserConfigureResponse
        {
            public IDisplayDisclosureConfig[] DisclosureConfigs { get; set; }
        }



        public class DisplayDisclosureConfig:IDetailedDisplayDisclosureConfig
        {
            public string ConfigString { get; set; }

            public string Property { get; set; }

            public string DisplayConfigureName { get;set; }
        }

        public class DisplayDisclosureConfigRequest : AjaxRequestModelBase,IDisplayDisclosureConfig
        {
            public string ConfigString { get; set; }

            public string Property { get; set; }
        }
    }

    public interface IDisplayDisclosureConfig
    {
        string ConfigString { get; set; }

        string Property { get; set; }
    }

    public interface IDetailedDisplayDisclosureConfig:IDisplayDisclosureConfig
    {
        string DisplayConfigureName { get; set; }
    }
}
