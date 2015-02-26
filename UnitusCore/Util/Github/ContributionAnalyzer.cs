using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Octokit;
using Octokit.Internal;

namespace UnitusCore.Util.Github
{
    public class ContributionAnalysis
    {
        private readonly GitHubClient _client;

        private User targetUser;

        public int AdditionCount { get; set; }

        public int DeletionCount { get; set; }

        public int CommitCount { get; set; }

        public int RepositoryCount { get; set; }

        public int StaredCount { get; set; }

        public int ForkedCount { get; set; }

        public int ForkingCount { get; set; }

        public int RepositoryCountWithOtherCommitter { get; set; }

        public int RepositoryCountWithOtherCommitterAndAuthority { get; set; }

        private LanguageDictionary ByLanguageInstance { get; set; }

        public CollaboratorDictionary Collaborators { get; set; }

        public IDictionary<string, ContributionAnalysisByLanguage> ByLanguage
        {
            get
            {
                return ByLanguageInstance;
            }
        }

        public ContributionAnalysis(GitHubClient client)
        {
            _client = client;
            ByLanguageInstance=new LanguageDictionary();
            Collaborators=new CollaboratorDictionary();
        }

        public static async Task<ContributionAnalysis> GetContributionAnalysis(GitHubClient client,IEnumerable<GithubAssociationManager.GithubRepositoryIdentity> identities)
        {
            ContributionAnalysis analysis=new ContributionAnalysis(client);
            await analysis.FetchAll(identities);
            return analysis;
        }

        public async Task FetchAll(IEnumerable<GithubAssociationManager.GithubRepositoryIdentity> identities)
        {
            targetUser = await _client.User.Current();
            //タスクブロックの定義
            var firstBlock
                = new TransformBlock<GithubAssociationManager.GithubRepositoryIdentity, ObtainedContributionInfo>(
                    async identity =>
                    {
                        IEnumerable<Contributor> contributors=null;
                        IEnumerable<RepositoryLanguage> langugaes=null;
                        await Task.WhenAll(Task.Run(async () =>
                        {
                            contributors = await FetchContributors(identity);
                        }), Task.Run(async () =>
                        {
                            langugaes = await FetchLanguages(identity);
                        })
                            );
                        if (contributors == null || langugaes == null) return new ObtainedContributionInfo(null,null,null);
                        return new ObtainedContributionInfo(contributors, identity, langugaes);
                    });
            var secoundBlock = new ActionBlock<ObtainedContributionInfo>((info) =>
            {
                if(info.RepositoryContributors==null||info.RepositoryContributors.Count()==0)return;
                //レポジトリに対する処理
                StatForRepository(info.RepositoryIdentity.TargetRepository);
                int sumCommit = info.RepositoryContributors.Sum(a => a.Total);
                int maxCommit = info.RepositoryContributors.Max(a => a.Total);
                //Contributorごとの処理
                foreach (Contributor contributor in info.RepositoryContributors)
                {
                    if (contributor.Author.Login.Equals(targetUser.Login))
                    {//統計対象だった場合
                        if (sumCommit != contributor.Total)
                        {
                            RepositoryCountWithOtherCommitter++;//他の作業者と一緒に行っているレポジトリ
                            if (maxCommit == contributor.Total)
                            {
                                RepositoryCountWithOtherCommitterAndAuthority++;//その中で主導権を握っているもの
                            }
                        }
                        StatForTarget(contributor,info.RepositoryLanguage,contributor.Total/(double)sumCommit);
                    }else
                    {//統計対象じゃないコミッターだった場合
                        StatForNotTarget(contributor, info.RepositoryLanguage,contributor.Total/(double)sumCommit);
                    }
                }
            });
            firstBlock.LinkTo(secoundBlock, new DataflowLinkOptions()
            {
                PropagateCompletion = true
            });
            foreach (var ident in identities)
            {
                firstBlock.Post(ident);
            }
            firstBlock.Complete();
            secoundBlock.Completion.Wait();
        }

        private void StatForNotTarget(Contributor contributor, IEnumerable<RepositoryLanguage> repositoryLanguage,double ratio)
        {
            lock (this)
            {
                var collaboratorInfo = new CollaboratorAnalysis(contributor.Author.Login,1);
                foreach (RepositoryLanguage rl in repositoryLanguage)
                {
                   collaboratorInfo.LanguageDictionary.Add(rl.Name,(long) (rl.NumberOfBytes*ratio));
                }
                Collaborators.Add(collaboratorInfo);
            }
        }

        private void StatForRepository(Repository targetRepository)
        {
            lock (this)
            {
                RepositoryCount++;
                if (targetRepository.Fork) ForkingCount++;
                ForkedCount += targetRepository.ForksCount;
                StaredCount += targetRepository.StargazersCount;
            }
            string repoLanguage = targetRepository.Language;
            if (string.IsNullOrWhiteSpace(repoLanguage)) repoLanguage = "(分類不可)";
            lock(this.ByLanguageInstance)
            {
                ByLanguageInstance.Add(new ContributionAnalysisByLanguage(repoLanguage, 0, 0, 0, 1, 0, 0));
            }
        }

        /// <summary>
        /// 統計対象のContributorとして処理をします
        /// </summary>
        /// <param name="contributor"></param>
        /// <param name="repositoryLanguage"></param>
        /// <param name="d"></param>
        private void StatForTarget(Contributor contributor, IEnumerable<RepositoryLanguage> repositoryLanguage, double ratio)
        {
            int addition = 0;
            int deletion = 0;
            foreach (WeeklyHash wh in contributor.Weeks)
            {
                addition += wh.Additions;
                deletion += wh.Deletions;
            }
            lock (this)
            {
                this.CommitCount += contributor.Total;
                this.AdditionCount += addition;
                this.DeletionCount += deletion;
            }
            var repositoryLanguages = repositoryLanguage as RepositoryLanguage[] ?? repositoryLanguage.ToArray();
            long sumByte = repositoryLanguages.Sum(lang => lang.NumberOfBytes);
            lock (this.ByLanguageInstance)
            {
                foreach (RepositoryLanguage lang in repositoryLanguages)
                {
                    double langRatio = lang.NumberOfBytes/(double)sumByte;
                    ByLanguageInstance.Add(new ContributionAnalysisByLanguage(lang.Name,(int) (langRatio*addition),(int) (langRatio*deletion),(int) (langRatio*contributor.Total),0,lang.NumberOfBytes,(long) (lang.NumberOfBytes*ratio)));
                }
            }
        }

        private async Task<IEnumerable<RepositoryLanguage>> FetchLanguages(
            GithubAssociationManager.GithubRepositoryIdentity identity)
        {
            IEnumerable<RepositoryLanguage> languages = null;
            try
            {
                languages = await _client.Repository.GetAllLanguages(identity.OwnerName, identity.RepoName);
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
            catch (ArgumentNullException argnull)
            {
                return new RepositoryLanguage[0];
            }
            return languages;
        }

        private async Task<IEnumerable<Contributor>> FetchContributors(GithubAssociationManager.GithubRepositoryIdentity identity)
        {
            IEnumerable<Contributor> contributors;
            try
            {
                contributors =
                    await _client.Repository.Statistics.GetContributors(identity.OwnerName, identity.RepoName);
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
            return contributors;
        }

        public class ObtainedContributionInfo
        {
            public ObtainedContributionInfo(IEnumerable<Contributor> repositoryContributors, GithubAssociationManager.GithubRepositoryIdentity repositoryIdentity, IEnumerable<RepositoryLanguage> repositoryLanguage)
            {
                RepositoryContributors = repositoryContributors;
                RepositoryIdentity = repositoryIdentity;
                RepositoryLanguage = repositoryLanguage;
            }

            public IEnumerable<Contributor> RepositoryContributors { get; set; }
 
            public GithubAssociationManager.GithubRepositoryIdentity RepositoryIdentity { get; set; }

            public IEnumerable<RepositoryLanguage> RepositoryLanguage { get; set; } 
        }


        public class ContributionAnalysisByLanguage
        {
            public ContributionAnalysisByLanguage(string languageName, int additionCount, int deletionCount, int commitCount, int repositoryCount, long sumByte, long sumEstimatedByte)
            {
                LanguageName = languageName;
                AdditionCount = additionCount;
                DeletionCount = deletionCount;
                CommitCount = commitCount;
                RepositoryCount = repositoryCount;
                SumByte = sumByte;
                SumEstimatedByte = sumEstimatedByte;
            }

            public string LanguageName { get; set; }

            public int AdditionCount { get; set; }

            public int DeletionCount { get; set; }

            public int CommitCount { get; set; }

            public int RepositoryCount { get; set; }

            public long SumByte { get; set; }

            public long SumEstimatedByte { get; set; }
        }

        public class CollaboratorDictionary : CalculatableObjectDictionary<CollaboratorAnalysis>
        {
            protected override CollaboratorAnalysis Update(CollaboratorAnalysis oldEntity, CollaboratorAnalysis newEntity)
            {
                oldEntity.SumCollaborated++;
                foreach (var entity in newEntity.LanguageDictionary)
                {
                    oldEntity.LanguageDictionary.Add(entity.Key,entity.Value);
                }
                return oldEntity;
            }
        }


        public class CollaboratorAnalysis:IObjectDictionaryEntity
        {
            public CollaboratorAnalysis(string name, int sumCollaborated)
            {
                Name = name;
                SumCollaborated = sumCollaborated;
                LanguageDictionary = new CollaboratorLanguageDictionary();
            }

            public string Name { get; set; }

            public int SumCollaborated { get; set; }

            public CollaboratorLanguageDictionary LanguageDictionary { get; set; }
        }

        public class CollaboratorLanguageDictionary : Dictionary<string, long>
        {
            public new void Add(string langName,long count)
            {
                if (ContainsKey(langName))
                {
                    this[langName]+=count;
                }
                else
                {
                    base.Add(langName,count);
                }
            }
        }

        private class LanguageDictionary : Dictionary<string, ContributionAnalysisByLanguage>
        {
            public void Add(ContributionAnalysisByLanguage lang)
            {
                if (ContainsKey(lang.LanguageName))
                {
                    var affectedLang = this[lang.LanguageName];
                    affectedLang.RepositoryCount += lang.RepositoryCount;
                    affectedLang.AdditionCount += lang.AdditionCount;
                    affectedLang.DeletionCount += lang.DeletionCount;
                    affectedLang.CommitCount += lang.CommitCount;
                    affectedLang.SumByte += lang.SumByte;
                    affectedLang.SumEstimatedByte += lang.SumEstimatedByte;
                }
                else
                {
                    Add(lang.LanguageName,lang);
                }

            }
        }
    }
}