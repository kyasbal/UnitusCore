using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Migrations
{
    public class UserSeedConfiguration
    {
        public static string[] seedRoles=new string[] {GlobalConstants.AdminRoleName};

        public static string[] seedPasswords = new string[] {"Kyasbal08!" ,"Yumeha00kiku!"};
        public static string[] seedEmails=new string[] {"LimeStreem@gmail.com", "trytestjapan@gmail.com" };
        public static bool[] seedIsAdmin=new bool[] {true,true};

        public static void RunUserSeed(ApplicationDbContext context)
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            PermissionManager permissionManager = new PermissionManager(context, userManager);
            for (int i = 0; i < seedRoles.Length; i++)
            {
                permissionManager.SafePermissionGet(seedRoles[i]);
            }
            for (int i = 0; i < seedEmails.Length; i++)
            {
                if (userManager.FindByName(seedEmails[i]) == null)
                {
                    ApplicationUser user = new ApplicationUser();
                    user.Email = seedEmails[i];
                    user.UserName = seedEmails[i];
                    user.Id = Guid.NewGuid().ToString();
                    Person person = new Person();
                    person.GenerateId();
                    person.Email = user.Email;
                    user.PersonData = person;
                    userManager.Create(user, seedPasswords[i]);
                }
                if (seedIsAdmin[i] && !permissionManager.CheckPermission(GlobalConstants.AdminRoleName, seedEmails[i]))
                {
                    permissionManager.ApplyPermissionToUser(GlobalConstants.AdminRoleName, seedEmails[i]);
                    Debug.WriteLine("Applied administrator permission/target:{0}", seedEmails[i]);
                }

            }
        }
    }
}