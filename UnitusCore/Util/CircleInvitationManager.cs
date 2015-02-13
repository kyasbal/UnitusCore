using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using UnitusCore.Controllers;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Util
{
    public class CircleInvitationManager
    {
        private static bool CheckUserPermission(ApplicationDbContext context,ApplicationUser user,Circle circle)
        {
            //管理権限のあるサークルリストの取得
            var administratedCircles = context.Entry(user).Collection(a => a.AdministrationCircle);
            if(!administratedCircles.IsLoaded)administratedCircles.Load();
            return user.AdministrationCircle.Any(a=>a.Id.Equals(circle.Id));
        }

        private static bool CheckIsAlreadyCircleMember(ApplicationDbContext context, Circle circle,string mailAddr)
        {
            //メンバーリストがロード済みだったら
            var circleMemberState = context.Entry(circle).Collection(a => a.Members);
            if(!circleMemberState.IsLoaded)circleMemberState.Load();
            foreach (MemberStatus member in circle.Members)
            {
                var bindedUserStatus = context.Entry(member).Reference(a => a.TargetUser);
                if(!bindedUserStatus.IsLoaded)bindedUserStatus.Load();
                var identityUserStatus = context.Entry(member.TargetUser).Reference(a => a.ApplicationUser);
                if(!identityUserStatus.IsLoaded)identityUserStatus.Load();
            }
            return circle.Members.Any(a => a.TargetUser.ApplicationUser.UserName.Equals(mailAddr));
        }

        private static bool CheckEmailAvailable(ApplicationDbContext context,Circle circle,string mailAddr)
        {
            //メール招待リストの取得
            var emailInvitationsState = context.Entry(circle).Collection(e => e.MemberInvitations);
            if (!emailInvitationsState.IsLoaded) emailInvitationsState.Load();
            if (!circle.MemberInvitations.Any(a => a.EmailAddress.Equals(mailAddr)))//既に送られている招待は存在しない
            {
                return true;
            }
            else
            {
                var invitation=circle.MemberInvitations.FirstOrDefault(a => a.EmailAddress.Equals(mailAddr));
                if (DateTime.Now - invitation.SentDate > new TimeSpan(0, 0, 30, 0))
                {
                    circle.MemberInvitations.Remove(invitation);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static string SetInvitationData(ApplicationDbContext context,ApplicationUser user,Circle circle,string mailAddr)
        {
            string confirmationId = IdGenerator.GetId(20);
            CircleMemberInvitation invitation=new CircleMemberInvitation();
            invitation.GenerateId();
            invitation.InvitedPerson = user.PersonData;
            invitation.InvitedCircle = circle;
            invitation.SentDate = DateTime.Now;
            invitation.ConfirmationKey = confirmationId;
            invitation.EmailAddress = mailAddr;
            context.CircleInvitations.Add(invitation);
            context.SaveChanges();
            return confirmationId;
        }

        public static CircleInvitationResult SendCircleInvitation(UnitusApiController controller,Circle circle, string[] mailAddrs)
        {
            var context = controller.DbSession;
            var user = controller.CurrentUser;
            if (CheckUserPermission(context, user, circle))
            {
                CircleInvitationResult result=new CircleInvitationResult();
                foreach (string mailAddr in mailAddrs)
                {
                    if (!CheckIsAlreadyCircleMember(context, circle, mailAddr))
                    {
                        if (CheckEmailAvailable(context, circle, mailAddr))
                        {
                            string confirmationId = SetInvitationData(context, user, circle, mailAddr);
                            SendGridManager.SendUseTemplate(mailAddr, TemplateType.CircleInvitation,
                                new Dictionary<string, string>()
                                {
                                    {"-inviter-",user.PersonData.Name},
                                    {"-circlename-",circle.Name},
                                    {"-mailaddr-",mailAddr},
                                    {"-linkaddr-",controller.Url.Link("Default",new{controller="CircleInvitationAccept",action="Index",confirmationId=confirmationId})}
                                });
                        }
                        else
                        {
                            result.HasWarning = true;
                            result.Warnings.Add(string.Format("{0}に連続で招待を送ることはできません。招待メールを同じ人に送る場合、30分以上開ける必要があります。", mailAddr));
                        }
                    }
                    else
                    {
                        result.HasWarning = true;
                        result.Warnings.Add(string.Format("{0}は既にサークルのメンバーとして登録されています。",mailAddr));
                    }
                }
                return result;
            }
            else
            {
                return new CircleInvitationResult(false, "権限が無効です。",false);
            }
        }
    }

    public class CircleInvitationResult
    {
        public CircleInvitationResult()
        {
            Warnings=new List<string>();
        }
        public CircleInvitationResult(bool suceess, string errorMessage,bool hasWarning)
        {
            Suceess = suceess;
            ErrorMessage = errorMessage;
            HasWarning = hasWarning;
            Warnings=new List<string>();
        }

        public bool Suceess { get; set; }

        public bool HasWarning { get; set; }

        public string ErrorMessage { get; set; }

        public List<string> Warnings { get; set; } 
    }
}