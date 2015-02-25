using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Octokit;
using UnitusCore.Storage.DataModels;

namespace UnitusCore.Util.Github
{
    public class GistAnalyzer
    {
        private readonly GitHubClient _client;

        public GistStatisticsForSingleUser StatisticsForUser { get; set; }

        private GistAnalyzer(GitHubClient client)
        {
            _client = client;
        }

        public async static Task<GistAnalyzer> GetGistAnalysis(GitHubClient client,string userId)
        {
            GistAnalyzer analyzer=new GistAnalyzer(client);
            var gists=await client.Gist.GetAll();
            analyzer.StatisticsForUser = new GistStatisticsForSingleUser(userId, 0, 0, 0, 0);
            foreach (Gist gist in gists)
            {
                analyzer.StatisticsForUser.SumGistCount++;
                analyzer.StatisticsForUser.SumSize += gist.Files.Sum(a => a.Value.Size);
                analyzer.StatisticsForUser.SumComments += gist.Comments;
                analyzer.StatisticsForUser.SumForked +=gist.Forks==null?0: gist.Forks.Count;
            }
            return analyzer;
        }



    }
}