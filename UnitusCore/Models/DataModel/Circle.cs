using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class Circle : ModelBaseWithTimeLogging
    {
        public Circle()
        {
            Events = new HashSet<Event>();
            Projects = new HashSet<Project>();
            Achivements = new HashSet<Achivement>();
            Members = new HashSet<MemberStatus>();
            CircleStatistises = new HashSet<CircleStatistics>();
            Administrators=new HashSet<ApplicationUser>();
            MemberInvitations=new HashSet<CircleMemberInvitation>();
            UploadedEntities=new HashSet<CircleUploaderEntity>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MemberCount { get; set; }

        public string WebAddress { get; set; }

        public string BelongedSchool { get; set; }

        public ICollection<Event> Events { get; set; }//binded

        public ICollection<Project> Projects { get; set; }//binded

        public ICollection<Achivement> Achivements { get; set; }//binded

        public ICollection<MemberStatus> Members { get; set; }//binded

        public string Notes { get; set; }

        public string Contact { get; set; }

        public bool CanInterCollege { get; set; }

        public string ActivityDate { get; set; }

        public ICollection<CircleStatistics> CircleStatistises { get; set; }//binded

        public ICollection<ApplicationUser>  Administrators { get; set; }//binded

        public ICollection<CircleMemberInvitation> MemberInvitations { get; set; } //binded

        public ICollection<CircleUploaderEntity> UploadedEntities { get; set; }//binded 


        public async static Task<Circle> FromIdAsync(ApplicationDbContext dbContext, string circleId, bool allowNotFound = false)
        {
            Guid circleGuid;
            if (!Guid.TryParse(circleId, out circleGuid))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            else
            {
                Circle circle = await dbContext.Circles.FindAsync(circleGuid);
                if(circle==null&&!allowNotFound)throw new HttpResponseException(HttpStatusCode.NotFound);
                return circle;
            }
        }

        public async Task<bool> HaveAuthority(ApplicationUser user,ApplicationDbContext dbContext)
        {
            await LoadAdministrators(dbContext);
            return Administrators.Any(a => a.Id.Equals(user.Id));
        }

        public async Task LoadMemberInvitations(ApplicationDbContext dbContext)
        {
            var invitationStatus = dbContext.Entry(this).Collection(a => a.MemberInvitations);
            if (!invitationStatus.IsLoaded) await invitationStatus.LoadAsync();
        }

        public async Task LoadMembers(ApplicationDbContext dbContext)
        {
            var memberStatus = dbContext.Entry(this).Collection(a => a.Members);
            if (!memberStatus.IsLoaded) await memberStatus.LoadAsync();
        }

        public async Task LoadAdministrators(ApplicationDbContext dbContext)
        {
            var memberStatus = dbContext.Entry(this).Collection(a => a.Administrators);
            if (!memberStatus.IsLoaded) await memberStatus.LoadAsync();
        }

        public async Task<IEnumerable<string>> GetMemberUserIds(ApplicationDbContext dbContext)
        {
            await this.LoadMembers(dbContext);
            HashSet<string> userIds=new HashSet<string>();
            foreach (MemberStatus member in this.Members)
            {
                await member.LoadReferencesAsync(dbContext);
                await member.TargetUser.LoadApplicationUser(dbContext);
                userIds.Add(member.TargetUser.ApplicationUser.Id);
            }
            return userIds;
        }

    }
}