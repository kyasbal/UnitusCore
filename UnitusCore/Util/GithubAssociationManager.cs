using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity;
using MvcContrib;
using Octokit;
using Octokit.Internal;
using UnitusCore.Controllers;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.DataModels;
using UnitusCore.Util.Github;
using Authorization = Octokit.Authorization;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace UnitusCore.Util
{
    public class GithubAssociationManager
    {
        public static string[] requireScopes = {"repo:status","user","repo","gist"};

        private Dictionary<string,GitHubClient> clientCache=new Dictionary<string, GitHubClient>(); 

        private ApplicationDbContext applicationDbContext;

        private ApplicationUserManager userManager;

        private ProductHeaderValue applicationHeaderValue;

        public GithubAssociationManager(ApplicationDbContext applicationDbContext, ApplicationUserManager userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.applicationHeaderValue=new ProductHeaderValue("UNITUS-CORE","1.00");
        }

        public GitHubClient GetAuthenticatedClientFromToken(string token)
        {
            if (clientCache.ContainsKey(token)) return clientCache[token];
            var github = new GitHubClient(applicationHeaderValue,
                new InMemoryCredentialStore(new Credentials(token)));
            clientCache.Add(token, github);
            return github;
        }

        public GitHubClient GetAuthenticatedClient(string userName)
        {
            var user = userManager.FindByName(userName);
            return GetAuthenticatedClient(user);
        }

        public GitHubClient GetAuthenticatedClient(ApplicationUser user)
        {
            return GetAuthenticatedClientFromToken(user.GithubAccessToken);
        }

        private Int32 GetCurrentWeekHead()
        {
            var d = DateTime.Now;
            DateTime today=new DateTime(d.Year,d.Month,d.Day,0,0,0);
            for (int i = 0; i < 7; i++)
            {
                if (today.DayOfWeek == DayOfWeek.Sunday) break;
                today-=TimeSpan.FromDays(1);
            }
            return (Int32) (today.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }

        

        public async Task<bool> IsAssociationEnabled(ApplicationUser user)
        {
            return await IsAssociationEnabled(user.GithubAccessToken);
        }

        public async Task<bool> IsAssociationEnabled(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken) || accessToken.Length != 40) return false;
            else
            {
                try
                {
                    var client = GetAuthenticatedClientFromToken(accessToken);
                    var user = await client.User.Current();
                    return true;
                }
                catch (RateLimitExceededException rlee)
                {
                    return true;
                }
            }
        }


        public async Task<string> GetAvatarUri(string userName)
        {
            var user = userManager.FindByName(userName);
            if (await IsAssociationEnabled(user.GithubAccessToken))
            {
                try
                {
                    var github = GetAuthenticatedClient(userName);
                    var githubUser = await github.User.Current();
                    return githubUser.AvatarUrl;
                }catch(RateLimitExceededException rlee)
                {
                    return "https://core.unitus-ac.com/Uploader/Download?imageId=jlTKlJU7xTQwaEH5";
                }
            }
            else
            {
                return "https://core.unitus-ac.com/Uploader/Download?imageId=jlTKlJU7xTQwaEH5";
            }

        }

        public async Task<int> GetRepositoryCount(GitHubClient client)
        {
            var user = await client.User.Current();
            return user.PublicRepos + user.OwnedPrivateRepos;
        }

        public async Task<IEnumerable<GithubRepositoryIdentity>> GetAllRepositoriesList(GitHubClient client)
        {
            var repositories = await client.Repository.GetAllForCurrent();
            HashSet<GithubRepositoryIdentity> resultData=new HashSet<GithubRepositoryIdentity>();
            foreach (Repository repositoryinfo in repositories)
            {
                resultData.Add(new GithubRepositoryIdentity(repositoryinfo.Owner.Login, repositoryinfo.Name,repositoryinfo));
            }
            return resultData;
        }

        private void AppendLangStatistics(ConcurrentDictionary<string, int> dict, string lang, int coefficient)
        {
            if (string.IsNullOrWhiteSpace(lang))
            {
                lang = "(分類不可)";
            }
            if (dict.ContainsKey(lang))
            {
                dict[lang] += coefficient;
            }
            else
            {
                dict.TryAdd(lang, coefficient);
            }
        }

        public async Task<ContributionAnalysis> GetAllRepositoryCommit(GitHubClient client, ContributeStatisticsByDay contributeStatistics)
        {
            return await ContributionAnalysis.GetContributionAnalysis(client, await GetAllRepositoriesList(client));
        }

        public async Task GetGists(GitHubClient client)
        {
            var g=await client.Gist.GetAll();
        }

        private double CalcSum(IDictionary<string, int> init)
        {
            int sumval = 0;
            foreach (KeyValuePair<string, int> data in init)
            {
                sumval += data.Value;
            }
            return sumval;
        }

        public class GithubCollaboratorStatisticData
        {
            public GithubCollaboratorStatisticData()
            {
                LanguageCommitcount=new Dictionary<string, int>();
                SumCommit = 0;
                SumAddition = 0;
                SumDeletion = 0;
            }
            public DateTime StatisticDateTime { get; set; }

            public int StatisticCount { get; set; }

            public int SumCommit { get; set; }

            public override string ToString()
            {
                return string.Format("StatisticDateTime: {0}, StatisticCount: {1}, SumCommit: {2}, SumAddition: {3}, SumDeletion: {4}, RepositoryCount: {5}, LanguageCommitcount: {6}", StatisticDateTime, StatisticCount, SumCommit, SumAddition, SumDeletion, RepositoryCount, LanguageCommitcount);
            }

            public int SumAddition { get; set; }

            public int SumDeletion { get; set; }

            public int RepositoryCount { get; set; }

            public Dictionary<string,int> LanguageCommitcount { get; set; }

            public ConcurrentDictionary<string, ConcurrentDictionary<string, int>> CollaboratorLog { get; set; }

            public int SumForked { get; set; }

            public int SumForking { get; set; }

            public int SumStared { set;get; }
        }
        

        public class GithubRepositoryIdentity
        {
            public GithubRepositoryIdentity(string ownerName, string repoName, Repository targetRepository)
            {
                OwnerName = ownerName;
                RepoName = repoName;
                TargetRepository = targetRepository;
            }

            public string OwnerName { get; set; }

            public string RepoName { get; set; }

            public Repository TargetRepository { get; set; }
        }
    }


}