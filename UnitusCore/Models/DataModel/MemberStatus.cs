using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class MemberStatus : ModelBase
    {
        public string Occupation { get; set; }

        public bool IsActiveMember { get; set; }

        public Person TargetUser { get; set; }//binded

        public Circle TargetCircle { get; set; }//binded

        public async Task LoadReferencesAsync(ApplicationDbContext dbContext)
        {
            var memberTargetUserStatus = dbContext.Entry(this).Reference(a => a.TargetUser);
            if (!memberTargetUserStatus.IsLoaded) await memberTargetUserStatus.LoadAsync();
            var userTargetStatus = dbContext.Entry(this.TargetUser).Reference(a => a.ApplicationUser);
            if (!userTargetStatus.IsLoaded) await userTargetStatus.LoadAsync();
            var circleStatus = dbContext.Entry(this).Reference(a => a.TargetCircle);
            if (!circleStatus.IsLoaded) await circleStatus.LoadAsync();
        }
    }
}