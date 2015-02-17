using System;
using System.Collections.Generic;
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

        public async void LoadMemberInvitations(ApplicationDbContext dbContext)
        {
            var invitationStatus = dbContext.Entry(this).Collection(a => a.MemberInvitations);
            if (!invitationStatus.IsLoaded) await invitationStatus.LoadAsync();
        }

    }
}