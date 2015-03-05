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

        public GistByLanguageDictionary ByLanguages { get; set; }

        private GistAnalyzer(GitHubClient client)
        {
            _client = client;
            ByLanguages = new GistByLanguageDictionary();
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
                foreach (KeyValuePair<string, GistFile> filePair in gist.Files)
                {
                    var gistFile = filePair.Value;
                    string language = string.IsNullOrWhiteSpace(gistFile.Language) ? "(分類不可)" : gistFile.Language;
                    analyzer.ByLanguages.Add(new GistStatisticsForSingleUserByLanguage(userId,language.ToSafeForTable(),gistFile.Size));
                }
            }
            return analyzer;
        }

        public class GistByLanguageDictionary : CalculatableObjectDictionary<GistStatisticsForSingleUserByLanguage>
        {
            protected override GistStatisticsForSingleUserByLanguage Update(GistStatisticsForSingleUserByLanguage oldEntity,
                GistStatisticsForSingleUserByLanguage newEntity)
            {
                oldEntity.FileInBytes += newEntity.FileInBytes;
                return oldEntity;
            }
        }
    }
}