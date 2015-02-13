using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Util
{
    public class CircleDatabaseHelper
    {
        public static async Task<IEnumerable<MemberStatus>> GetBelongingCircle(ApplicationDbContext context,ApplicationUser user)
        {
            DbReferenceEntry<ApplicationUser, Person> personReferenceState = context.Entry(user).Reference(a=>a.PersonData);
            if (!personReferenceState.IsLoaded)await personReferenceState.LoadAsync();
            DbCollectionEntry<Person, MemberStatus> circleReferenceState = context.Entry(user.PersonData).Collection(a => a.BelongedCircles);
            if (!circleReferenceState.IsLoaded) await circleReferenceState.LoadAsync();
            HashSet<MemberStatus> returnData=new HashSet<MemberStatus>();
            foreach (var memberStatus in circleReferenceState.CurrentValue)
            {
                DbReferenceEntry<MemberStatus, Circle> targetCircleReferenceState =
                    context.Entry(memberStatus).Reference(a => a.TargetCircle);
                if (!targetCircleReferenceState.IsLoaded) await targetCircleReferenceState.LoadAsync();
                returnData.Add(memberStatus);
            }
            return returnData;
        }

        public static async Task<bool> CheckHaveAuthorityAboutCircle(ApplicationDbContext context,ApplicationUser user,Circle targetCircle)
        {
            var administratorsReferenceState = context.Entry(targetCircle).Collection(a => a.Administrators);
            if (!administratorsReferenceState.IsLoaded) await administratorsReferenceState.LoadAsync();
            return targetCircle.Administrators.Contains(user);
        }
    }
}