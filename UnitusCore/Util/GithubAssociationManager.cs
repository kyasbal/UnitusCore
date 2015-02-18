using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
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
                resultData.Add(new GithubRepositoryIdentity(repositoryinfo.Owner.Login, repositoryinfo.Name,repositoryinfo));
            }
            return resultData;
        }

        private void AppendLangStatistics(ConcurrentDictionary<string, int> dict, string lang, int coefficient)
        {
            if (string.IsNullOrWhiteSpace(lang)) return;
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
            Stopwatch st=new Stopwatch();
            st.Start();
            var user = await client.User.Current();
            var repoIdentities = await GetAllRepositoriesList(client);
            GithubCollaboratorStatisticData statistic=new GithubCollaboratorStatisticData();
            statistic.StatisticDateTime = DateTime.Now;
            int repositoryCount=0;
            IEnumerable<GithubRepositoryIdentity> githubRepositoryIdentities = repoIdentities as IList<GithubRepositoryIdentity> ?? repoIdentities.ToList();
            statistic.RepositoryCount = githubRepositoryIdentities.Count();
            ConcurrentDictionary<string, int> commitLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> additionLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> deletionLangDictionary = new ConcurrentDictionary<string, int>();
            ConcurrentDictionary<string, int> repositoryLangDictionary = new ConcurrentDictionary<string, int>();
            //全レポジトリの情報を取得
            var GetContributerBlock = new TransformBlock<GithubRepositoryIdentity, Tuple<IEnumerable<Contributor>,GithubRepositoryIdentity>>(
                async ident =>
                {
                    return new Tuple<IEnumerable<Contributor>,GithubRepositoryIdentity>(await client.Repository.Statistics.GetContributors(ident.OwnerName, ident.RepoName),ident);
                },new ExecutionDataflowBlockOptions()
                {
                    MaxDegreeOfParallelism =DataflowBlockOptions.Unbounded
                });
            //追加する
            var sumActionBlock = new ActionBlock<Tuple<IEnumerable<Contributor>,GithubRepositoryIdentity>>(initData =>
            {
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
                }
                Console.WriteLine(" a:{0} d{1} c{2}", a, d, c);
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
            st.Stop();
           WriteDict(commitLangDictionary);
           WriteDict(additionLangDictionary);
           WriteDict(deletionLangDictionary);
           WriteDict(repositoryLangDictionary);
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
            Console.WriteLine(statistic);
            Console.WriteLine(st.ElapsedMilliseconds);
            return statistic;
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

        private void WriteDict(IDictionary<string, int> init)
        {
            int sumval = 0;
            foreach (KeyValuePair<string, int> data in init)
            {
                sumval += data.Value;
            }

            foreach (KeyValuePair<string, int> data in init)
            {
                Console.WriteLine("{0}:{1}  {2}%",data.Key,data.Value,data.Value/(double)sumval*100d);
            }
            Console.WriteLine();
        }
////
//
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