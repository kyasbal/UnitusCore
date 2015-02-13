using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.SqlServer.Server;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Models
{
    // ApplicationUser クラスにプロパティを追加することでユーザーのプロファイル データを追加できます。詳細については、http://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。


    public static class AccountExtensionMethods
    {

        public static IdentityResult CreateUser(this ApplicationUserManager userManager,ApplicationDbContext dbContext,string email, string password)
        {
            ApplicationUser user = new ApplicationUser();
            user.Id = Guid.NewGuid().ToString();
            user.Email = email;
            user.UserName = email;
            Person p=new Person();
            p.GenerateId();
            p.Email = email;
            p.ApplicationUser = user;
            user.PersonData = p;
            dbContext.SaveChanges();
            return userManager.Create(user, password);
        }
    }

}