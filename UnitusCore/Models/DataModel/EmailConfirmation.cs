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
                ConfirmationId = confirmationId,
                TargetUser = targetUser
            };
            confirmation.GenerateId();
            return confirmation;
        }
        public ApplicationUser TargetUser { get; set; }//binded
        [Index]
        public string ConfirmationId { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}