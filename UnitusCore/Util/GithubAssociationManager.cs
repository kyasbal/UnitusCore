using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MvcContrib;
using Octokit;
using Octokit.Internal;
using UnitusCore.Controllers;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Util
{
    public class GithubAssociationManager
    {
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
            var github = new GitHubClient(applicationHeaderValue,
                new InMemoryCredentialStore(new Credentials(token)));
            return github;
        }

        public GitHubClient GetAuthenticatedClient(string userName)
        {
            var user = userManager.FindByName(userName);
            var github = new GitHubClient(applicationHeaderValue,
                new InMemoryCredentialStore(new Credentials(user.GithubAccessToken)));
            return github;
        }

        public GitHubClient GetAuthenticatedClient(ApplicationUser user)
        {
            var github = new GitHubClient(applicationHeaderValue,
                new InMemoryCredentialStore(new Credentials(user.GithubAccessToken)));
            return github;
        }

        public bool IsAssociationEnabled(ApplicationUser user)
        {
            return IsAssociationEnabled(user.GithubAccessToken);
        }

        public bool IsAssociationEnabled(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length != 40) return false;
            else
            {
                return true;
            }
        }


        public async Task<string> GetAvatarUri(string userName)
        {
            if (IsAssociationEnabled(userName))
            {
                var github = GetAuthenticatedClient(userName);
                var user = await github.User.Current();
                return user.AvatarUrl;
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
                resultData.Add(new GithubRepositoryIdentity(repositoryinfo.Owner.Login, repositoryinfo.Name));
            }
            return resultData;
        }

//
//
        public async Task<GithubCollaboratorStatisticData> GetAllRepositoryCommit(GitHubClient client)
        {
            Stopwatch st=new Stopwatch();
            st.Start();
            var user = await client.User.Current();
            var repoIdentities = await GetAllRepositoriesList(client);
            GithubCollaboratorStatisticData statistic=new GithubCollaboratorStatisticData();
            statistic.RepositoryCount = repoIdentities.Count();
            foreach (var r in repoIdentities)
            {
                var contributers =
                    await client.Repository.Statistics.GetContributors(r.OwnerName, r.RepoName);
                foreach (var contributor in contributers)
                {
                    if (contributor.Author.Login.Equals(user.Login))
                    {
                        statistic.SumCommit += contributor.Total;

                        break;
                    }
                }
            }
            st.Stop();
            Console.WriteLine(st.ElapsedMilliseconds);
            return statistic;
        }
////
//
        public class GithubCollaboratorStatisticData
        {
            public GithubCollaboratorStatisticData()
            {
                LanguageCommitcount=new Dictionary<string, int>();
            }

            public int SumCommit { get; set; }
            
            public int SumAddition { get; set; }

            public int SumDeletion { get; set; }

            public int RepositoryCount { get; set; }

            public Dictionary<string,int> LanguageCommitcount { get; set; } 
        }
        

        public class GithubRepositoryIdentity
        {
            public GithubRepositoryIdentity(string ownerName, string repoName)
            {
                OwnerName = ownerName;
                RepoName = repoName;
            }

            public string OwnerName { get; set; }

            public string RepoName { get; set; }
        }
    }


}