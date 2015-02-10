using System;
using System.ComponentModel.DataAnnotations.Schema;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class EmailConfirmation :ModelBase
    {
        public static EmailConfirmation GenerateEmailConfirmation(ApplicationUser targetUser,string confirmationId)
        {
            EmailConfirmation confirmation=new EmailConfirmation()
            {
                ExpireTime = DateTime.Now+new TimeSpan(7,0,0,0),
                UserInfo = targetUser,
                ConfirmationId = confirmationId,
                TargetUserIdentifyCode = Guid.Parse(targetUser.Id)
            };
            confirmation.GenerateId();
            return confirmation;
        }
        [Index]
        public Guid TargetUserIdentifyCode { get; set; }
        public string ConfirmationId { get; set; }
        public ApplicationUser UserInfo { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}