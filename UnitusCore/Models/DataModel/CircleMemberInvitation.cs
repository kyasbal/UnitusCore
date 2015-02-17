using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using UnitusCore.Models.BaseClasses;
using UnitusCore.Util;

namespace UnitusCore.Models.DataModel
{
    public class CircleMemberInvitation : ModelBase
    {
        public Circle InvitedCircle { get; set; }//binded

        public Person InvitedPerson { get; set; }//binded

        public string ConfirmationKey { get; set; }

        public string EmailAddress { get; set; }

        public DateTime SentDate { get; set; }

        public async Task LoadReferences(ApplicationDbContext dbContext)
        {
            var circleLoadStatus = dbContext.Entry(this).Reference(a => a.InvitedCircle);
            if (!circleLoadStatus.IsLoaded) await circleLoadStatus.LoadAsync();
            var personLoadStatus = dbContext.Entry(this).Reference(a => a.InvitedPerson);
            if (!personLoadStatus.IsLoaded) await personLoadStatus.LoadAsync();
        }

        public async static Task<CircleMemberInvitation> FindFromIdAsync(ApplicationDbContext dbContext,string targetInvitationId,bool allowNotFound=false)
        {
            Guid id = targetInvitationId.ToValidGuid();
            var result=await dbContext.CircleInvitations.FindAsync(id);
            if(!allowNotFound&&result==null)throw new HttpResponseException(HttpStatusCode.NotFound);
            return result;
        }
    }
}