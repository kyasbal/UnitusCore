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
            var client = manager.GetAuthenticatedClientFromToken("b1526091539cf5d2ddf9b0076b68c80138cb08b8");
            var user = await client.User.Current();
            var auth=await client.Authorization.Update(user.Id, new AuthorizationUpdate());
            var token=await manager.IsAssociationEnabled("731dc5b4f7d2346f28751b00436df0178402a2a2");
            await manager.GetGists(client);
        }
    }
}
