using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using Octokit;
using UnitusCore;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.DataModels;
using UnitusCore.Util;

namespace UnitusCoreUnitTest
{
    [TestFixture]
    public class GithubAssociationTest
    {
        private ApplicationDbContext DbSession;

        private ApplicationUserManager UserManager;

        [TestFixtureSetUp]
        public void PreTest()
        {
            DbSession =
                new ApplicationDbContext(
                    "Data Source=localhost;Initial Catalog=UnitusCoreTesting;Integrated Security=True;MultipleActiveResultSets=true;");
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(DbSession));
            ApplicationUserManager.SetValidator(UserManager);
            manager = new GithubAssociationManager(DbSession,UserManager);
        }


        private GithubAssociationManager manager;

        [Test]
        public async void RepositoryCommitTest()
        {
            var client = manager.GetAuthenticatedClientFromToken("731dc5b4f7d2346f28751b00436df0178402a2a2");
            ContributeStatisticsByDay contributeStatistics = ContributeStatisticsByDay.GenerateTodayStatistics("UNIT TEST");
            var data=await manager.GetAllRepositoryCommit(client, contributeStatistics);
            
        }

        [Test]
        public async void GistTest()
        {
            var client = manager.GetAuthenticatedClientFromToken("731dc5b4f7d2346f28751b00436df0178402a2a2");
            var user = await client.User.Current();
            var repos=await manager.GetAllRepositoriesList(client);
            foreach (var githubRepositoryIdentity in repos)
            {
                Console.WriteLine("{0}/{1}", githubRepositoryIdentity.OwnerName, githubRepositoryIdentity.RepoName);
                if (client.Repository == null)
                {
                    Console.WriteLine("repository is null");
                    continue;
                }
                try
                {
                    var source =
                        await
                            client.Repository.GetAllLanguages(githubRepositoryIdentity.OwnerName,
                                githubRepositoryIdentity.RepoName);
                    if (source == null)
                    {
                        Console.WriteLine("source is null");
                        continue;
                    }
                    foreach (var allLanguage in source)
                    {
                        Console.WriteLine("{0}  ---{1} bytes", allLanguage.Name, allLanguage.NumberOfBytes);
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("cant fetch data");
                }
            }
            await manager.GetGists(client);
        }
    }
}
