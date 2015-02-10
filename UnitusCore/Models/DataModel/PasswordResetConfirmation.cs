using System;
using System.ComponentModel.DataAnnotations.Schema;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
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
}