using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.SqlServer.Server;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Util
{
    public static class MailConfirmationManager
    {
        private static string seedCharactors = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static string generateEmailConfirmationId()
        {
            string resultStr = "";
            Random rand=new Random();
            for (int i = 0; i < 20; i++)
            {
                resultStr += seedCharactors[rand.Next(0, seedCharactors.Length)];
            }
            return resultStr;
        }

        private static int getRandomIndex(ApplicationDbContext context)
        {
            Random rand = new Random();
            while (true)
            {
                int index = rand.Next();
                if (!context.EmailConfirmations.Any(e => e.KeyIndex == index))
                {
                    return index;
                }
            }
        }



        private static bool CheckMailServiceAvailable(ApplicationDbContext dbContext,ApplicationUser user)
        {
            foreach (EmailConfirmation confirmations in user.SentConfirmations)
            {
                var delta = DateTime.Now - (confirmations.ExpireTime-new TimeSpan(7,0,0,0));//終了時刻から、送信時刻を図る
                if (delta.TotalMinutes < 5)
                {
                    return false;
                }
            }
            return true;
        }

        private static void RemoveOtherMailConfirmationData(ApplicationDbContext dbContext,ApplicationUser user)
        {
            Guid userIdentityCode = Guid.Parse(user.Id);
            HashSet<EmailConfirmation> removeCandidate=new HashSet<EmailConfirmation>();
            foreach (EmailConfirmation confirmation in user.SentConfirmations)
            {
                removeCandidate.Add(confirmation);
            }
            dbContext.EmailConfirmations.RemoveRange(removeCandidate);
            dbContext.SaveChanges();
        }



        public static SendMailConfirmationResult SendMailConfirmation(ApplicationUser user,Controller controller)
        {
            ApplicationDbContext context = controller.Request.GetOwinContext().Get<ApplicationDbContext>();
            if (!CheckMailServiceAvailable(context, user))
                return new SendMailConfirmationResult(false, "メールの再送信は連続的にはできません。前の送信から5分ほど開けて再度試行してみてください。");
            RemoveOtherMailConfirmationData(context,user);
            string confirmationID = generateEmailConfirmationId();
            EmailConfirmation confirm = EmailConfirmation.GenerateEmailConfirmation(getRandomIndex(context),user,confirmationID);
            context.EmailConfirmations.Add(confirm);
            context.SaveChanges();
            SendGridManager.SendUseTemplate(user.Email,TemplateType.AccountConfirmation,new Dictionary<string, string>()
            {
                {"-username-",user.Email},
                {"-linkaddr-",  controller.Url.Action("MailConfirm","Account",new {confirmId=confirmationID},controller.Request.Url.Scheme)}
            });
            return new SendMailConfirmationResult(true,"Successful Completion");
        }

        public static ConfirmationResult ConfirmConfirmationId(string confirmationId,Controller controller)
        {
            ApplicationDbContext context = controller.Request.GetOwinContext().Get<ApplicationDbContext>();
            var confirmationData =
                context.EmailConfirmations.Where(c => c.ConfirmationId.Equals(confirmationId)).FirstOrDefault();
            if (confirmationData != null)
            {
                if (confirmationData.ExpireTime > DateTime.Now)
                {
                    var user = confirmationData.TargetUser;
                    user.EmailConfirmed = true;
                    context.EmailConfirmations.Remove(confirmationData);
                    context.SaveChanges();
                    SendGridManager.SendUseTemplate(user.Email,TemplateType.AccountConfirmationCompleted,new Dictionary<string, string>{ {"-username-",user.UserName} });
                    return new ConfirmationResult(user.UserName, true, "Successful Completion");
                }
                else
                {
                    context.EmailConfirmations.Remove(confirmationData);
                    context.SaveChanges();
                }
            }
                return new ConfirmationResult("",false,"無効なトークンが渡されました。");
        }
    }

    public class ConfirmationResult
    {
        public ConfirmationResult(string userName, bool success, string errorMessage)
        {
            UserName = userName;
            Success = success;
            ErrorMessage = errorMessage;
        }

        public string UserName { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get;set; }
    }

    public class SendMailConfirmationResult
    {
        public SendMailConfirmationResult()
        {
            
        }

        public SendMailConfirmationResult(bool success, string errormessage)
        {
            Success = success;
            Errormessage = errormessage;
        }

        public bool Success { get; set; }

        public string Errormessage { get; set; }
    }
}