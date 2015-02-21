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
                var client = GetAuthenticatedClientFromToken(accessToken);
                var user =await client.User.Current();
                return true;
            }
        }


        public async Task<string> GetAvatarUri(string userName)
        {
            var user = userManager.FindByName(userName);
            if (await IsAssociationEnabled(user.GithubAccessToken))
            {
                var github = GetAuthenticatedClient(userName);
                var githubUser = await github.User.Current();
                return githubUser.AvatarUrl;
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

        public async Task<GithubCollaboratorStatisticData> GetAllRepositoryCommit(GitHubClient client, ContributeStatisticsByDay contributeStatistics)
        {
            var user = await client.User.Current();
            var repoIdentities = await GetAllRepositoriesList(client);
            GithubCollaboratorStatisticData statistic=new GithubCollaboratorStatisticData();
            statistic.StatisticDateTime = DateTime.Now;
            IEnumerable<GithubRepositoryIdentity> githubRepositoryIdentities = repoIdentities as IList<GithubRepositoryIdentity> ?? repoIdentities.ToList();
            statistic.RepositoryCount = githubRepositoryIdentities.Count();
            ConcurrentDictionary<string, int> commitLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> additionLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> deletionLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> repositoryLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string,ConcurrentDictionary<string,int>> collaboratorLog=new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();
            //全レポジトリの情報を取得
            var GetContributerBlock = new TransformBlock<GithubRepositoryIdentity, Tuple<IEnumerable<Contributor>,GithubRepositoryIdentity>>(
                async ident =>
                {
                    try
                    {
                        //Console.WriteLine("{0}/{1} will be esitimated.", ident.OwnerName, ident.RepoName);
                        var d =
                            new Tuple<IEnumerable<Contributor>, GithubRepositoryIdentity>(
                                await client.Repository.Statistics.GetContributors(ident.OwnerName, ident.RepoName),
                                ident);
                        Console.WriteLine("{0}/{1},○", ident.OwnerName, ident.RepoName);
                        return d;
                    }
                    catch (ApiException apiEx)
                    {
                        if (apiEx.StatusCode == HttpStatusCode.NoContent)
                        {
                            return null;
                        }
                        else
                        {
                            throw;
                        }
                    }
                },new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism =DataflowBlockOptions.Unbounded
                });
            //追加する
            var sumActionBlock = new ActionBlock<Tuple<IEnumerable<Contributor>,GithubRepositoryIdentity>>(initData =>
            {
                
                if (initData == null) return;
                if(initData.Item2.TargetRepository.Fork)contributeStatistics.SumForking++;
                contributeStatistics.SumForked += initData.Item2.TargetRepository.ForksCount;
                contributeStatistics.SumStared += initData.Item2.TargetRepository.StargazersCount;
                var contributors = initData.Item1;
                int c = 0, a = 0, d = 0;
                foreach (var contributor in contributors)
                {

                    if (contributor.Author.Login.Equals(user.Login))
                    {
                        c += contributor.Total;
                        foreach (WeeklyHash w in contributor.Weeks)
                        {
                            a += w.Additions;
                            d += w.Deletions;
                        }
                        break;
                    }
                    else
                    {
                        string contributorLogin = contributor.Author.Login;
                        string langName = initData.Item2.TargetRepository.Language;
                        if (string.IsNullOrWhiteSpace(langName)) langName = "(分類不可)";
                        if (collaboratorLog.ContainsKey(contributorLogin))
                        {
                            if (collaboratorLog[contributorLogin].ContainsKey(langName))
                            {
                                collaboratorLog[contributorLogin][langName]++;
                            }
                            else
                            {
                                collaboratorLog[contributorLogin].TryAdd(langName, 1);
                            }
                        }
                        else
                        {
                            collaboratorLog.TryAdd(contributorLogin,new ConcurrentDictionary<string, int>());
                            if (collaboratorLog[contributorLogin].ContainsKey(langName))
                            {
                                collaboratorLog[contributorLogin][langName]++;
                            }
                            else
                            {
                                collaboratorLog[contributorLogin].TryAdd(langName, 1);
                            }
                        }
                    }
                }
                lock (statistic)
                {
                    AppendLangStatistics(commitLangDictionary,initData.Item2.TargetRepository.Language,c);
                    AppendLangStatistics(additionLangDictionary, initData.Item2.TargetRepository.Language, a);
                    AppendLangStatistics(deletionLangDictionary, initData.Item2.TargetRepository.Language, d);
                    AppendLangStatistics(repositoryLangDictionary, initData.Item2.TargetRepository.Language, 1);
                    statistic.SumAddition += a;
                    statistic.SumCommit += c;
                    statistic.SumDeletion += d;
                }
            });
            statistic.CollaboratorLog = collaboratorLog;
            GetContributerBlock.LinkTo(sumActionBlock, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });
            foreach (var ident in githubRepositoryIdentities)
            {
                GetContributerBlock.Post(ident);
            }
            GetContributerBlock.Complete();
            sumActionBlock.Completion.Wait();
            double aSum = CalcSum(additionLangDictionary);
            double dSum = CalcSum(deletionLangDictionary);
            double cSum = CalcSum(commitLangDictionary);
            double rSum = CalcSum(repositoryLangDictionary);
            foreach (KeyValuePair<string, int> pairs in commitLangDictionary)
            {
                contributeStatistics.LanguageStatistics.Add(
                    new SingleUserLanguageStatisticsByDay(contributeStatistics.UniqueId, pairs.Key,
                        additionLangDictionary[pairs.Key], deletionLangDictionary[pairs.Key],
                        commitLangDictionary[pairs.Key], repositoryLangDictionary[pairs.Key],
                        additionLangDictionary[pairs.Key]/aSum, deletionLangDictionary[pairs.Key]/dSum,
                        commitLangDictionary[pairs.Key]/cSum, repositoryLangDictionary[pairs.Key]/rSum));
            }
            Console.WriteLine("Forking{0},Forked{1}",contributeStatistics.SumForking,contributeStatistics.SumForked);
            return statistic;
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