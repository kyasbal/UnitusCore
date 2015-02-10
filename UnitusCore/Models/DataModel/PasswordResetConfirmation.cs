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
            confirm.TargetUser = user;
            confirm.ExpireTime = DateTime.Now + new TimeSpan(0, 0, 30, 0);
            confirm.ConfirmationId = confirmationId;
            return confirm;
        }

        public string ConfirmationId { get; set; }

        public ApplicationUser TargetUser { get; set; }

        public DateTime ExpireTime { get; set; }
    }
}