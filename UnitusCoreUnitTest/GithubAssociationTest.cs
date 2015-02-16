using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using UnitusCore;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
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
            var client = manager.GetAuthenticatedClientFromToken("b47f54c7f2afb543f421ce7a01a4a7db6bb72140");
            var data = await manager.GetAllRepositoryCommit(client);
            Console.WriteLine(data.ToString());
        }
    }
}
