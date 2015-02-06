using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace UnitusCore.Models
{
    // ApplicationUser クラスにプロパティを追加することでユーザーのプロファイル データを追加できます。詳細については、http://go.microsoft.com/fwlink/?LinkID=317594 を参照してください。
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            AccessableCircles=new HashSet<Circle>();   
            Permissions=new HashSet<UserPermission>(); 
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

    public class UserPermission : ModelBase
    {
        public UserPermission()
        {
            AllowedUsers=new HashSet<ApplicationUser>();
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
    }
}