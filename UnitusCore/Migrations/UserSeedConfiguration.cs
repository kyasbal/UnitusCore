using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using UnitusCore.Models;

namespace UnitusCore.Migrations
{
    public class UserSeedConfiguration
    {
        public static string[] seedRoles=new string[] {GlobalConstants.AdminRoleName};

        public static string[] seedUsernames=new string[] {"LimeStreem"};
        public static string[] seedPasswords = new string[] {"Kyasbal08!" };
        public static string[] seedEmails=new string[] {"LimeStreem@gmail.com"};
        public static bool[] seedIsAdmin=new bool[] {true};

        public static void RunUserSeed(ApplicationDbContext context)
        {
            ApplicationUserManager userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            RoleManager<IdentityRole> roleManager=new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            for (int i = 0; i < seedRoles.Length; i++)
            {
                if (!roleManager.RoleExists(seedRoles[i]))
                {
                    roleManager.Create(new IdentityRole(seedRoles[i]));
                }
            }
            for (int i = 0; i < seedUsernames.Length; i++)
            {
                if (userManager.FindByName(seedUsernames[i]) == null)
                {
                    ApplicationUser user=new ApplicationUser();
                    user.Email = seedEmails[i];
                    user.UserName = seedEmails[i];
                    user.Id = Guid.NewGuid().ToString();
                    userManager.Create(user, seedPasswords[i]);
                    if (seedIsAdmin[i]) userManager.AddToRole(user.Id, GlobalConstants.AdminRoleName);
                }
            }
        }
    }
}