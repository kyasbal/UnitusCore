using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Util
{
    public class PasswordForgotManager
    {
        private static string seedCharactors = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static string generateEmailConfirmationId()
        {
            string resultStr = "";
            Random rand = new Random();
            for (int i = 0; i < 20; i++)
            {
                resultStr += seedCharactors[rand.Next(0, seedCharactors.Length)];
            }
            return resultStr;
        }

        private static int getRandomIndex(ApplicationDbContext context)
        {
            Random rand=new Random();
            while (true)
            {
                int index = rand.Next();
                if (!context.PasswordResetConfirmations.Any(e => e.KeyIndex == index))
                {
                    return index;
                }
            }
        }


        private static bool CheckMailServiceAvailable(ApplicationDbContext dbContext, ApplicationUser user)
        {
            foreach (PasswordResetConfirmation confirmations in user.PasswordResetRequests)
            {
                var delta = DateTime.Now - (confirmations.ExpireTime - new TimeSpan(0, 0, 30, 0));//終了時刻から、送信時刻を図る
                if (delta.TotalMinutes < 15)
                {
                    return false;
                }
            }
            return true;
        }

        private static void RemoveOtherPasswordConfirmationData(ApplicationDbContext dbContext, ApplicationUser user)
        {
            HashSet<PasswordResetConfirmation> removeCandidate = new HashSet<PasswordResetConfirmation>();
            foreach (PasswordResetConfirmation confirmation in user.PasswordResetRequests)
            {
                removeCandidate.Add(confirmation);
            }
            dbContext.PasswordResetConfirmations.RemoveRange(removeCandidate);
            dbContext.SaveChanges();
        }

        public static SendPasswordResetResult SendPasswordResetMail(string emailAddr,Controller controller)
        {
            ApplicationDbContext context = controller.Request.GetOwinContext().Get<ApplicationDbContext>();
            ApplicationUserManager userManager =
                controller.Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            ApplicationUser targetUser = userManager.FindByEmail(emailAddr);
            if (targetUser != null)
            {
                if (CheckMailServiceAvailable(context, targetUser))
                {
                    RemoveOtherPasswordConfirmationData(context,targetUser);

                    string confirmationID = userManager.GeneratePasswordResetToken(targetUser.Id);
                    PasswordResetConfirmation confirmation =
                        PasswordResetConfirmation.GeneratePasswordResetConfirmation(getRandomIndex(context),targetUser, confirmationID);
                    context.PasswordResetConfirmations.Add(confirmation);
                    context.SaveChanges();
                    SendGridManager.SendUseTemplate(emailAddr, TemplateType.PasswordResetConfirmation,
                        new Dictionary<string, string>()
                        {
                            {"-username-",emailAddr},
                            {"-linkaddr-",controller.Url.Action("ForgotPasswordConfirm","Account",new {confirmId=confirmationID},controller.Request.Url.Scheme)}
                        });
                    
                    return new SendPasswordResetResult(true, "パスワードリセットメールを指定されたアドレスに送信しました。");
                }
                else
                {
                    return new SendPasswordResetResult(false, "パスワードリセットメールは連続して送れません。15分ほど開けてお試しください。");
                }
            }
            else
            {
                return new SendPasswordResetResult(false,"指定されたユーザー名は存在しません。");
            }
        }

        public static ConfirmationResult ConfirmConfirmationId(string confirmationId, Controller controller,bool needRemoveoldKey)
        {
            ApplicationDbContext context = controller.Request.GetOwinContext().Get<ApplicationDbContext>();
            var confirmationData =
                context.PasswordResetConfirmations.Where(c => c.ConfirmationId.Equals(confirmationId)).Include(a => a.TargetUser).FirstOrDefault();
            if (confirmationData != null)
            {
                if (confirmationData.ExpireTime > DateTime.Now)
                {
                    var user = confirmationData.TargetUser;
                    if (needRemoveoldKey)
                    {
                        context.PasswordResetConfirmations.Remove(confirmationData);
                        context.SaveChanges();
                    }
                    return new ConfirmationResult(user.UserName, true, "Successful Completion");
                }
                else
                {
                    context.PasswordResetConfirmations.Remove(confirmationData);
                    context.SaveChanges();
                }
            }
            return new ConfirmationResult("", false, "無効なトークンが渡されました。");
        }
    }


    public class SendPasswordResetResult
    {
        public SendPasswordResetResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}