using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using NUnit.Framework;
using UnitusCore;
using UnitusCore.Controllers;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCoreUnitTest
{
    [TestFixture]
    public class CircleInvitationTest
    {
        private void CreateUserIfNotexist(string userName, string password)
        {
            if(UserManager.FindByName(userName)!=null)return;
            UserManager.CreateUser(DbSession, userName, password);
        }

        private Circle getCircleFromCirclename(string circleName)
        {
           Circle cir=DbSession.Circles.Where(e => e.Name.Equals(circleName)).FirstOrDefault();
            if (cir != null)
            {
                DbSession.Entry(cir).Collection(e=>e.Members).Load();
            }
            return cir;
        }

        private void CreateCircleIfNotExist(string circleName, string userName)
        {
            if (DbSession.Circles.Where(e => e.Name.Equals(circleName)).FirstOrDefault() == null)
            {
                ApplicationUser user = UserManager.FindByName(userName);
                Circle circle=new Circle();
                circle.GenerateId();
                circle.Name = circleName;
                circle.Administrators.Add(user);
                DbSession.Circles.Add(circle);
                DbSession.SaveChanges();
                AddAsMember(circleName,userName);
            }
        }

        private void AddAsMember(string circleName, string username)
        {
            Circle cir = getCircleFromCirclename(circleName);
            ApplicationUser user = UserManager.FindByName(username);
            DbSession.Entry(user).Reference(e => e.PersonData).Load();
            foreach (MemberStatus memberStatuse in cir.Members)
            {
                DbSession.Entry(memberStatuse).Reference(e=>e.TargetUser).Load();
                if (memberStatuse.TargetUser.Id.Equals(user.PersonData.Id))
                {
                    return;
                }
            }
            MemberStatus st=new MemberStatus();
            st.GenerateId();
            st.TargetCircle = cir;
            st.TargetUser = user.PersonData;
            DbSession.MemberStatuses.Add(st);
            DbSession.SaveChanges();
        }

        [TestFixtureSetUp]
        public void PreTest()
        {
            DbSession =
                new ApplicationDbContext(
                    "Data Source=localhost;Initial Catalog=UnitusCoreTesting;Integrated Security=True;MultipleActiveResultSets=true;");
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(DbSession));
            
            ApplicationUserManager.SetValidator(UserManager);
           CreateUserIfNotexist("circleowner@gmail.com", "CircleOwner1234!");
           CreateUserIfNotexist("circlemember@gmail.com", "CircleMember1234!");
            CreateCircleIfNotExist("テストサークル","circleowner@gmail.com");
            AddAsMember("テストサークル","circlemember@gmail.com");
        }

        [Test]
        public void TestCreateSecurityHash()
        {
            string generatedId1=IdGenerator.GetId(20);
            string generatedId2 = IdGenerator.GetId(20);
            Assert.IsFalse(generatedId1==generatedId2);
            Assert.IsTrue(
                CircleInvitationAcceptController.createSecurityHash("circleowner@gmail.com", generatedId1)==
                CircleInvitationAcceptController.createSecurityHash("circleowner@gmail.com", generatedId1));
            Assert.IsFalse(
    CircleInvitationAcceptController.createSecurityHash("circleowner2@gmail.com", generatedId1)==
    CircleInvitationAcceptController.createSecurityHash("circleowner@gmail.com", generatedId1));
            Assert.IsFalse(
    CircleInvitationAcceptController.createSecurityHash("circleowner@gmail.com", generatedId2)==
    CircleInvitationAcceptController.createSecurityHash("circleowner@gmail.com", generatedId1));
        }

        [Test]
        [TestCase("circleowner@gmail.com", "テストサークル", true)]
        [TestCase("circleowner@gmail.com", "テストサークル2", false)]
        [TestCase("circlemember@gmail.com", "テストサークル", false)]
        public void TestCircleAdministration(string userName,string circleName,bool intendToHavePermission)
        {
            ApplicationUser user = UserManager.FindByName(userName);
            Circle cir = getCircleFromCirclename(circleName);
            Assert.IsTrue(CircleInvitationManager.CheckUserPermission(DbSession,user,cir)==intendToHavePermission);
        }


        

        public ApplicationDbContext DbSession { get; set; }

        public ApplicationUserManager UserManager { get; set; }


    }
}
