using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UnitusCore.Models.DataModel
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            AccessableCircles = new HashSet<Circle>();
            Permissions = new HashSet<UserPermission>();
            AdministrationCircle=new HashSet<Circle>();
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // authenticationType が CookieAuthenticationOptions.AuthenticationType で定義されているものと一致している必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // ここにカスタム ユーザー クレームを追加します
            return userIdentity;
        }
        public string GithubAccessToken { get; set; }

        public Person PersonData { get; set; }

        public ICollection<Circle> AdministrationCircle { get; set; } 

        public ICollection<Circle> AccessableCircles { get; set; }

        public ICollection<UserPermission> Permissions { get; set; }
    }
}