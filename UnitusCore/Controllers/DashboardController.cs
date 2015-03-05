using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using Octokit;
using UnitusCore.Attributes;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Storage;
using UnitusCore.Storage.Base;
using UnitusCore.Storage.DataModels.Profile;
using UnitusCore.Util;
using WebGrease.Css.Extensions;


namespace UnitusCore.Controllers
{
    
    public class DashboardController : UnitusApiControllerWithTableConnection
    {
        [ApiAuthorized]
        [UnitusCorsEnabled]
        [HttpGet]
        [Route("Dashboard")]
        public async Task<IHttpActionResult> GetDashboardStatus(string ValidationToken)
        {
            return await this.OnValidToken(ValidationToken, async () =>
            {
                return await GetDashboardResponse(CurrentUserWithPerson);
            });
        }

        [ApiAuthorized]
        [UnitusCorsEnabled]
        [HttpGet]
        [Route("Dashboard")]
        [RoleRestrict("Administrator")]
        public async Task<IHttpActionResult> GetDashboardStatus(string ValidationToken,string userName)
        {
            return await this.OnValidToken(ValidationToken, async () =>
            {
                return await GetDashboardResponse(await UserManager.FindByNameAsync(userName));
            });
        }

        private async Task<IHttpActionResult> GetDashboardResponse(ApplicationUser user)
        {
            await user.LoadPersonData(DbSession);
            PermissionManager permission = new PermissionManager(DbSession, UserManager);
            GithubAssociationManager manager = new GithubAssociationManager(DbSession, UserManager);
            AchivementStatisticsStorage achivementStatisticsStorage = new AchivementStatisticsStorage(new TableStorageConnection(), DbSession);
            return Json(ResultContainer<GetDashboardStatusAjaxResponse>.GenerateSuccessResult(
                new GetDashboardStatusAjaxResponse(
                    permission.CheckPermission("Administrator",user.UserName),
                    user.PersonData.Name,
                    user.Email,
                    await manager.GetAvatarUri(user.Email),
                    (await GetAsArrayOfResultCircleData(await CircleDatabaseHelper.GetBelongingCircle(DbSession, user)))
                    , await GetUserProfile(CurrentUser), (await achivementStatisticsStorage.GetAchivementCategories()).ToArray())));
        }


        //[UnitusCorsEnabled]
        //[HttpGet]
        //[AllowAnonymous]
        //[Route("Dashboard/Dummy")]
        //public async Task<IHttpActionResult> GetDashboardStatusDummy(string validationToken)
        //{
        //    AchivementStatisticsStorage achivementStatisticsStorage = new AchivementStatisticsStorage(new TableStorageConnection(), DbSession);
        //    return await this.OnValidToken(validationToken, async () =>
        //    {
        //        return Json(ResultContainer<GetDashboardStatusAjaxResponse>.GenerateSuccessResult(
        //            new GetDashboardStatusAjaxResponse(
        //                true,
        //                "種市 朝日",
        //                "darafu@gmail.com",
        //                "https://avatars2.githubusercontent.com/u/230541?v=3&s=460",
        //                new CircleBelongingStatus[] { new CircleBelongingStatus("ダラフ株式会社", "HelloID", true), new CircleBelongingStatus("EspicaCompute", "IDIDIDIDIDIDIDIDIDIID", false), },
        //                new DetailedProfile("東京理科大学", "理工学部", "電気電子工学科", Person.Cource.MC1, new GithubProfile(true, 29),new UserConfig(false),""),(await achivementStatisticsStorage.GetAchivementCategories()).ToArray()))
        //            );
        //    });
        //}

        private async Task<CircleBelongingStatus[]> GetAsArrayOfResultCircleData(IEnumerable<MemberStatus> status)
        {
            HashSet<CircleBelongingStatus> result = new HashSet<CircleBelongingStatus>();
            foreach (var st in status)
            {
                var circleBelongingStatus = new CircleBelongingStatus(st.TargetCircle.Name, st.TargetCircle.Id.ToString(),
                    await
                        CircleDatabaseHelper.CheckHaveAuthorityAboutCircle(DbSession, CurrentUserWithPerson,
                            st.TargetCircle));
                CircleTagStorage stroage = new CircleTagStorage(new TableStorageConnection());
                circleBelongingStatus.CircleTags = (await stroage.RetrieveTagBodies(circleBelongingStatus.CircleId,circleBelongingStatus.HasAuthority)).Select(a => a.Tag);
                result.Add(circleBelongingStatus);
            }
            return result.ToArray();
        }


        private async Task<DetailedProfile> GetUserProfile(ApplicationUser targetUser)
        {
            return
                await
                    DetailedProfile.GenerateFromUser(targetUser, DbSession, TableConnection, AccessBy.Owner,
                        await GetGithubProfile());
        }

        private async Task<GithubProfile> GetGithubProfile()
        {
            GithubAssociationManager associationManager = new GithubAssociationManager(DbSession, UserManager);
            if (await associationManager.IsAssociationEnabled(CurrentUser))
            {
                GitHubClient client = associationManager.GetAuthenticatedClient(CurrentUser);
                if (client == null) return new GithubProfile(false, 0);
                return new GithubProfile(true, await associationManager.GetRepositoryCount(client));
            }
            else
            {
                return new GithubProfile(false, 0);
            }
        }

        public class GetDashboardStatusAjaxResponse
        {
            public GetDashboardStatusAjaxResponse(bool isAdministrator, string name, string userName, string avatarUri, CircleBelongingStatus[] circleBelonging, DetailedProfile profile, string[] achivementCategories)
            {
                IsAdministrator = isAdministrator;
                Name = name;
                UserName = userName;
                AvatarUri = avatarUri;
                CircleBelonging = circleBelonging;
                Profile = profile;
                AchivementCategories = achivementCategories;
            }

            public bool IsAdministrator { get; set; }

            public string Name { get; set; }

            public string UserName { get; set; }

            public string AvatarUri { get; set; }

            public DetailedProfile Profile { get; set; }

            public CircleBelongingStatus[] CircleBelonging { get; set; }

            public string[] AchivementCategories { get; set; }
        }

        public class CircleBelongingStatus
        {
            public CircleBelongingStatus(string circleName, string circleId, bool hasAuthority)
            {
                CircleName = circleName;
                CircleId = circleId;
                HasAuthority = hasAuthority;
            }

            public string CircleName { get; set; }

            public string CircleId { get; set; }

            public bool HasAuthority { get; set; }

            public IEnumerable<string> CircleTags { get; set; } 
        }

        public class DetailedProfile:IProtectedSchoolInfoContainer
        {
            public static async Task<DetailedProfile> GenerateFromUser(ApplicationUser user,ApplicationDbContext dbContext,TableStorageConnection storageConnection,AccessBy accessBy,GithubProfile profile)
            {
                await user.LoadPersonData(dbContext);
                var disclosureConfig = new ProfileDisclosureConfigStorage(storageConnection, user.Id);
                var skillProfileStroage = new SkillProfileStorage(storageConnection);
                var p = user.PersonData;
                DetailedProfile result = new DetailedProfile()
                {
                    Email = await disclosureConfig.FetchProtectedProperty(ProfileProperty.MailAddress,accessBy,()=>p.Email),
                    Url = await disclosureConfig.FetchProtectedProperty(ProfileProperty.Url, accessBy,()=>p.Url),
                    BelongedSchool = await disclosureConfig.FetchProtectedProperty(ProfileProperty.University, accessBy,()=>p.BelongedSchool),
                    Faculty = await disclosureConfig.FetchProtectedProperty(ProfileProperty.Faculty, accessBy,()=>p.Faculty),
                    Major = await disclosureConfig.FetchProtectedProperty(ProfileProperty.Major, accessBy,()=>p.Major),
                    Skills = await disclosureConfig.FetchProtectedProperty(ProfileProperty.Language,accessBy, () => skillProfileStroage.GetAllSkillProfile(user.Id).Select(Mapper.DynamicMap<ISkillProfile,SkillProfileContainer>)),
                    CurrentGrade = p.CurrentCource,
                    Notes = p.Notes,
                    GithubProfile = profile,
                    CreatedDateInfo = p.CreationDate.ToString("yyyy年M月d日に登録"),
                    CreatedDateInfoByDateOffset = string.Format("{0}日目",(int)(DateTime.Now-p.CreationDate).TotalDays)
                };

                return result;
            }

            public DetailedProfile()
            {
                
            }

            public GithubProfile GithubProfile { get; set; }

            public DisclosureProtectedResponse BelongedSchool { get; set; }

            public DisclosureProtectedResponse Faculty { get; set; }

            public DisclosureProtectedResponse Major { get; set; }

            public DisclosureProtectedResponse Email { get; set; }

            public DisclosureProtectedResponse Url { get; set; }

            public DisclosureProtectedResponse Skills { get; set; }

            public Person.Cource CurrentGrade { get; set; }

            public string Notes { get; set; }

            public string CreatedDateInfo { get; set; }

            public string CreatedDateInfoByDateOffset { get; set; }
        }

        public class GithubProfile
        {
            public GithubProfile(bool associationEnabled, int repositoryCount)
            {
                AssociationEnabled = associationEnabled;
                RepositoryCount = repositoryCount;
            }

            public bool AssociationEnabled { get; set; }

            public int RepositoryCount { get; set; }
        }

        public class UserConfig
        {
            public UserConfig(bool showOwnProfileToOtherCircle)
            {
                ShowOwnProfileToOtherCircle = showOwnProfileToOtherCircle;
            }

            public bool ShowOwnProfileToOtherCircle { get; set; }
        }
    }
}