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
            Permissions = new HashSet<UserPermission>();
            AdministrationCircle = new HashSet<Circle>();
            SentConfirmations = new HashSet<EmailConfirmation>();
            PasswordResetRequests=new HashSet<PasswordResetConfirmation>();
        }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // authenticationType が CookieAuthenticationOptions.AuthenticationType で定義されているものと一致している必要があります
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // ここにカスタム ユーザー クレームを追加します
            return userIdentity;
        }
        public string GithubAccessToken { get; set; }

        public Person PersonData { get; set; }//binded

        public ICollection<Circle> AdministrationCircle { get; set; } //binded

        public ICollection<UserPermission> Permissions { get; set; }//binded

        public ICollection<EmailConfirmation> SentConfirmations { get; set; }//binded 

        public ICollection<PasswordResetConfirmation> PasswordResetRequests { get; set; }//binded 
    }
}