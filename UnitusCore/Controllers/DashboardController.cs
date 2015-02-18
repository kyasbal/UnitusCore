using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using Octokit;
using UnitusCore.Attributes;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Results;
using UnitusCore.Util;
using WebGrease.Css.Extensions;


namespace UnitusCore.Controllers
{
    
    public class DashboardController : UnitusApiController
    {
        [ApiAuthorized]
        [UnitusCorsEnabled]
        [HttpGet]
        [Route("Dashboard")]
        public async Task<IHttpActionResult> GetDashboardStatus(string ValidationToken)
        {
            return await this.OnValidToken(ValidationToken, async () =>
            {
                PermissionManager permission = new PermissionManager(DbSession, UserManager);
                GithubAssociationManager manager = new GithubAssociationManager(DbSession, UserManager);

                return Json(ResultContainer<GetDashboardStatusAjaxResponse>.GenerateSuccessResult(
                    new GetDashboardStatusAjaxResponse(
                        permission.CheckPermission("Administrator", User.Identity.Name),
                        CurrentUserWithPerson.PersonData.Name,
                        CurrentUserWithPerson.Email,
                        await manager.GetAvatarUri(CurrentUserWithPerson.Email),
                        (await GetAsArray(await CircleDatabaseHelper.GetBelongingCircle(DbSession, CurrentUserWithPerson)))
                        , await GetUserProfile())));
            });
        }


        [UnitusCorsEnabled]
        [HttpGet]
        [AllowAnonymous]
        [Route("Dashboard/Dummy")]
        public async Task<IHttpActionResult> GetDashboardStatusDummy(string validationToken)
        {
            return await this.OnValidToken(validationToken, async () =>
            {
                return Json(ResultContainer<GetDashboardStatusAjaxResponse>.GenerateSuccessResult(
                    new GetDashboardStatusAjaxResponse(
                        true,
                        "種市 朝日",
                        "darafu@gmail.com",
                        "https://avatars2.githubusercontent.com/u/230541?v=3&s=460",
                        new CircleBelongingStatus[] { new CircleBelongingStatus("ダラフ株式会社", "HelloID", true), new CircleBelongingStatus("EspicaCompute", "IDIDIDIDIDIDIDIDIDIID", false), },
                        new DetailedProfile("東京理科大学", "理工学部", "電気電子工学科", Person.Cource.MC1, new GithubProfile(true, 29),new UserConfig(false),"")))
                    );
            });
        }

        private async Task<CircleBelongingStatus[]> GetAsArray(IEnumerable<MemberStatus> status)
        {
            HashSet<CircleBelongingStatus> result = new HashSet<CircleBelongingStatus>();
            foreach (var st in status)
            {
                result.Add(new CircleBelongingStatus(st.TargetCircle.Name, st.TargetCircle.Id.ToString(),
                    await
                        CircleDatabaseHelper.CheckHaveAuthorityAboutCircle(DbSession, CurrentUserWithPerson,
                            st.TargetCircle)));
            }
            return result.ToArray();
        }

        private async Task<UserConfig> GetUserConfig()
        {
            var p = CurrentUserWithPerson.PersonData;
            await p.LoadUserConfigure(DbSession);
            return new UserConfig(p.UserConfigure.ShowOwnProfileToOtherCircle);
        }


        private async Task<DetailedProfile> GetUserProfile()
        {
            var p = CurrentUserWithPerson.PersonData;
            return new DetailedProfile(p.BelongedColledge, p.Faculty, p.Major, p.CurrentCource, await GetGithubProfile(),await GetUserConfig(),p.Notes);
        }

        private async Task<GithubProfile> GetGithubProfile()
        {
            GithubAssociationManager associationManager = new GithubAssociationManager(DbSession, UserManager);
            if (associationManager.IsAssociationEnabled(CurrentUser))
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
            public GetDashboardStatusAjaxResponse(bool isAdministrator, string name, string userName, string avatarUri, CircleBelongingStatus[] circleBelonging, DetailedProfile profile)
            {
                IsAdministrator = isAdministrator;
                Name = name;
                UserName = userName;
                AvatarUri = avatarUri;
                CircleBelonging = circleBelonging;
                Profile = profile;
            }

            public bool IsAdministrator { get; set; }

            public string Name { get; set; }

            public string UserName { get; set; }

            public string AvatarUri { get; set; }

            public DetailedProfile Profile { get; set; }

            public CircleBelongingStatus[] CircleBelonging { get; set; }
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
        }

        public class DetailedProfile
        {
            public DetailedProfile(string belongingColledge, string faculty, string major, Person.Cource currentGrade, GithubProfile githubProfie, UserConfig userConfigure, string notes)
            {
                BelongingColledge = belongingColledge;
                Faculty = faculty;
                Major = major;
                CurrentGrade = currentGrade;
                GithubProfie = githubProfie;
                UserConfigure = userConfigure;
                Notes = notes;
            }

            public GithubProfile GithubProfie { get; set; }

            public UserConfig UserConfigure { get; set; }

            public string BelongingColledge { get; set; }

            public string Faculty { get; set; }

            public string Major { get; set; }

            public Person.Cource CurrentGrade { get; set; }

            public string Notes { get; set; }
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