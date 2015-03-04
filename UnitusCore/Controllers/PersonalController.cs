using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using AutoMapper;
using UnitusCore.Attributes;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.DataModels.Profile;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class PersonalController:UnitusApiControllerWithTableConnection
    {
        [UnitusCorsEnabled]
        [ApiAuthorized]
        [HttpPut]
        [Route("Personal/Profile")]
        public async Task<IHttpActionResult> PutPersonalProfile(PutPersonalRequest req)
        {
            return await this.OnValidToken(req, async (a) =>
            {

                Person personData = CurrentUserWithPerson.PersonData;
                personData.BelongedSchool = req.BelongedSchool ?? personData.BelongedSchool;
                personData.Faculty = req.Faculty ?? personData.Faculty;
                personData.Major = req.Major ?? personData.Major;
                personData.CurrentCource = req.CurrentGrade;
                personData.Notes = req.Notes ?? personData.Notes;
                await DbSession.SaveChangesAsync();
                return Json(ResultContainer.GenerateSuccessResult());
            });
        }

        [RoleRestrict(GlobalConstants.AdminRoleName)]
        [UnitusCorsEnabled]
        [ApiAuthorized]
        [HttpPut]
        [Route("Personal/Profile/Admin")]
        public async Task<IHttpActionResult> PutPersonalProfileAdmin(PutPersonalRequestAdmin req)
        {
            return await this.OnValidToken(req, async (a) =>
            {
                var applicationUser = await UserManager.FindByNameAsync(req.UserName);
                await applicationUser.LoadPersonData(DbSession);
                Person personData = applicationUser.PersonData;
                personData.Name = req.Name ?? personData.Name;
                personData.BelongedSchool = req.BelongedSchool ?? personData.BelongedSchool;
                personData.Faculty = req.Faculty ?? personData.Faculty;
                personData.Major = req.Major ?? personData.Major;
                personData.CurrentCource = req.CurrentGrade;
                personData.Notes = req.Notes ?? personData.Notes;
                await DbSession.SaveChangesAsync();
                return Json(ResultContainer.GenerateSuccessResult());
            });
        }

        [UnitusCorsEnabled]
        [ApiAuthorized]
        [HttpPut]
        [Route("Personal/Profile/Skills")]
        public async Task<IHttpActionResult> PutSkillProfile(PutSkillProfileRequest profileRequest)
        {
            Ensure.NotEmptyString(profileRequest.SkillName);
            return await this.OnValidToken(profileRequest,async (r) =>
            {
                SkillProfileStorage sprofile = new SkillProfileStorage(TableConnection);
                await sprofile.AddOrReplaceProfile(CurrentUser.Id, r.SkillName, r.SkillLevel);
                return Json(ResultContainer.GenerateSuccessResult(GetSkillProfiles(CurrentUser.Id)));
            }
            );
        }

        public  IEnumerable<ISkillProfile> GetSkillProfiles(string userId)
        {
            SkillProfileStorage sprofile=new SkillProfileStorage(TableConnection);
            return sprofile.GetAllSkillProfile(userId).Select(Mapper.DynamicMap<ISkillProfile,SkillProfileContainer>);
        }

        public class PutSkillProfileRequest : AjaxRequestModelBase,ISkillProfile
        {
            public string SkillName { get; set; }

            public SkillLevel SkillLevel { get; set; }
        }

        public class PutPersonalRequestAdmin:PutPersonalRequest
        {
            public string UserName { get; set; }

            public string Name { get; set; }
        }

        public class PutPersonalRequest : AjaxRequestModelBase,IMajorInfoContainer
        {
            [Required]
            [MaxLength(32)]
            public string BelongedSchool { get; set; }
            [Required]
            [MaxLength(32)]
            public string Faculty { get; set; }
            [Required]
            [MaxLength(32)]
            public string Major { get; set; }
            [Required]
            public Person.Cource CurrentGrade { get; set; }
            [Required]
            [MaxLength(640)]
            public string Notes { get; set; }
        }
    }
}