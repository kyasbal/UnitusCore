using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using UnitusCore;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace UnitusCoreUnitTest
{
    [TestFixture]
    public class Account
    {
        private ApplicationDbContext DbSession;

        private ApplicationUserManager UserManager;

        [TestFixtureSetUp]
        public void PreTest()
        {
            DbSession =
                new ApplicationDbContext(
                    "Data Source=localhost;Initial Catalog=UnitusCoreTesting;Integrated Security=True;MultipleActiveResultSets=true;");
            UserManager=new ApplicationUserManager(new UserStore<ApplicationUser>(DbSession));
            ApplicationUserManager.SetValidator(UserManager);
        }

        [Test]
        public void DbSessionCheck()
        {
            Assert.IsNotNull(DbSession);
        }

        [Test]
        public void UserManagerCheck()
        {
            Assert.IsNotNull(UserManager);
        }

        [Test]
        [TestCase("LimeStreemTs@gmail.com","Kyasbal!",true)]
        [TestCase("LimeStreemTs", "Kyasbal!", false)]
        [TestCase("LimeStreemTs@gmail.com", "abcd", false)]
        public void UserCreationCheck(string userName,string password,bool intendToSuccess)
        {
            var d=UserManager.CreateUser(DbSession, userName, password);
            Console.WriteLine("Succeeded:{0}\nErrorMessages:{1}",d.Succeeded,d.Errors.CombineLines());
            Assert.IsTrue(intendToSuccess==d.Succeeded);
            var user = UserManager.FindByName(userName);
            if (user != null)
            {
                DbSession.Entry(user).Reference(a => a.PersonData).Load();
                DbSession.People.Remove(user.PersonData);
                DbSession.Users.Remove(user);
                DbSession.SaveChanges();
                Console.WriteLine("user{0} was removed",userName);
            }
        }

        [TestFixtureTearDown]
        public void PostTest()
        {

        }
    }
}
