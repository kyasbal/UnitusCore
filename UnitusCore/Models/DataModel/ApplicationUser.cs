using System.Collections.Generic;
using System.Linq;
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
            UploadedEntities=new HashSet<CircleUploaderEntity>();
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

        public ICollection<CircleUploaderEntity> UploadedEntities { get; set; }//binded 

        public async Task LoadPersonData(ApplicationDbContext dbSession)
        {
            var persondataStatus = dbSession.Entry(this).Reference(a => a.PersonData);
            if (!persondataStatus.IsLoaded)await persondataStatus.LoadAsync();
        }

        public async Task<IEnumerable<Circle>> RetrieveBelongingCircles(ApplicationDbContext dbSession)
        {
            await LoadPersonData(dbSession);
            var circleMemberStatus = dbSession.Entry(PersonData).Collection(a => a.BelongedCircles);
            if (!circleMemberStatus.IsLoaded) await circleMemberStatus.LoadAsync();
            HashSet<Circle> circleIds=new HashSet<Circle>();
            foreach (MemberStatus circleStatus in PersonData.BelongedCircles.ToArray())
            {
                await circleStatus.LoadReferencesAsync(dbSession);
                circleIds.Add(circleStatus.TargetCircle);
            }
            return circleIds;
        }

        public async Task LoadAdministrationCircles(ApplicationDbContext dbSession)
        {
            var administrationCircleStatus = dbSession.Entry(this).Collection(a => a.AdministrationCircle);
            if (!administrationCircleStatus.IsLoaded) await administrationCircleStatus.LoadAsync();
        }
    }
}