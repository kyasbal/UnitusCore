using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.SqlServer.Server;

namespace UnitusCore.Models
{
    // ApplicationUser クラスにプロパティを追加することでユーザーのプロファイル データを追加できます。詳細については、http://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            AccessableCircles = new HashSet<Circle>();
            Permissions = new HashSet<UserPermission>();
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // authenticationType が CookieAuthenticationOptions.AuthenticationType で定義されているものと一致している必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // ここにカスタム ユーザー クレームを追加します
            return userIdentity;
        }

        public Person AccessablePerson { get; set; }

        public ICollection<Circle> AccessableCircles { get; set; }

        public ICollection<UserPermission> Permissions { get; set; }
    }

    public class EmailConfirmation :ModelBase
    {
        public static EmailConfirmation GenerateEmailConfirmation(ApplicationUser targetUser,string confirmationId)
        {
            EmailConfirmation confirmation=new EmailConfirmation()
            {
                ExpireTime = DateTime.Now+new TimeSpan(7,0,0,0),
                UserInfo = targetUser,
                ConfirmationId = confirmationId,
                TargetUserIdentifyCode = Guid.Parse(targetUser.Id)
            };
            confirmation.GenerateId();
            return confirmation;
        }
        [Index]
        public Guid TargetUserIdentifyCode { get; set; }
        public string ConfirmationId { get; set; }
        public ApplicationUser UserInfo { get; set; }
        public DateTime ExpireTime { get; set; }
    }

    public class PasswordResetConfirmation : ModelBase
    {
        public static PasswordResetConfirmation GeneratePasswordResetConfirmation(ApplicationUser user,string confirmationId)
        {
            PasswordResetConfirmation confirm=new PasswordResetConfirmation();
            confirm.GenerateId();
            confirm.TargetUserIdentifyCode = Guid.Parse(user.Id);
            confirm.UserInfo = user;
            confirm.ExpireTime = DateTime.Now + new TimeSpan(0, 0, 30, 0);
            confirm.ConfirmationId = confirmationId;
            return confirm;
        }

        [Index]
        public Guid TargetUserIdentifyCode { get; set; }

        public string ConfirmationId { get; set; }

        public ApplicationUser UserInfo { get; set; }

        public DateTime ExpireTime { get; set; }
    }


    public class UserPermission : ModelBase
    {
        public UserPermission()
        {
            AllowedUsers = new HashSet<ApplicationUser>();
        }
        public string PermissionName { get; set; }

        public ICollection<ApplicationUser> AllowedUsers { get; set; }
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<UserPermission> Permissions { get; set; }

        public DbSet<EmailConfirmation> EmailConfirmations { get; set; }

        public DbSet<PasswordResetConfirmation> PasswordResetConfirmations { get; set; }
    }

    public static class AccountExtensionMethods
    {

        public static IdentityResult CreateUser(this ApplicationUserManager userManager,string email, string password)
        {
            ApplicationUser user = new ApplicationUser();
            user.Id = Guid.NewGuid().ToString();
            user.Email = email;
            user.UserName = email;
            return userManager.Create(user, password);
        }
    }

}