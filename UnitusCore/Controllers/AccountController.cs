using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SendGridSharp;
using UnitusCore.Attributes;
using UnitusCore.Models;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class AccountController : Controller
    {
        public ApplicationUserManager UserManager
        {
            get { return Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        public IAuthenticationManager AuthenticationContext
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        [AllowAnonymous]
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = null;
                if ((user = UserManager.Find(request.UserName, request.Password)) != null)
                {
                    var loginIdentity =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationContext.SignIn(new AuthenticationProperties() {IsPersistent = true}, loginIdentity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return loginFailedResult("ユーザー名またはパスワードが間違っています。");
                }
            }
            else
            {
                return loginFailedResult("不正なログイン情報が渡されました。");
            }
        }

        [Authorize]
        [RoleRestrict("Administrator")]
        [HttpGet]
        public async Task<ActionResult> AddAccount(AddAccountInfomationResponse response=null)
        {
            return View(response);
        }

        [Authorize]
        [HttpPost]
        [RoleRestrict("Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAccount(AddAccountRequest request)
        {
            if (ModelState.IsValid)
            {
                var state=UserManager.CreateUser(request.UserName, request.Password);
                if (state.Succeeded)
                {
                    if (request.IsAdmin)
                    {
                        Request.GetOwinContext().GetPermissionManager().ApplyPermissionToUser(GlobalConstants.AdminRoleName,request.UserName);
                    }
                    var user=UserManager.FindByName(request.UserName);
                    var result=MailConfirmationManager.SendMailConfirmation(user, this);
                    if (result.Success)
                    {
                        return accountCreationStateResult(string.Format("アカウント:{0}は正常に作成されました。<br>メールアドレスの確認のためのメールを{0}に送りました。", request.UserName), true);
                    }
                    else
                    {//メールが遅れなかった場合
                        return accountCreationStateResult(string.Format("アカウント:{0}は正常に作成されました。", request.UserName), true);
                    }

                }
                else
                {
                    return accountCreationStateResult(string.Format("アカウント:{0}の作成に失敗しました。<br>{1}", request.UserName,state.Errors.FirstOrDefault()),false);
                }
            }
            else
            {
                return
                    accountCreationStateResult(
                        string.Format("アカウント:{0}の作成に失敗しました。<br>不正なパラメータが渡されました。", request.UserName), false);
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> SendMailConfirm()
        {
            ApplicationUser user = UserManager.FindByName(User.Identity.Name);
            if (user != null)
            {
                if (!user.EmailConfirmed)
                {
                    var result=MailConfirmationManager.SendMailConfirmation(user, this);
                    if (result.Success)
                    {
                        return sendMailConfirmResult("確認メールの送信", "アカウントのメールアドレスに確認用のメールを送りました。", true);
                    }
                    else
                    {
                        return sendMailConfirmResult("エラー", result.Errormessage, false);
                    }

                }
                else
                {
                    return sendMailConfirmResult("エラー", "このメールアドレスは既に登録されています。<br>確認の必要はありません。", false);
                }

            }
            else
            {
                return sendMailConfirmResult("エラー", "ユーザーが正常に認識できませんでした。", false);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> MailConfirm(string confirmId)
        {
            var result = MailConfirmationManager.ConfirmConfirmationId(confirmId, this);
            if (result.Success)
            {
                return sendMailConfirmResult("認証完了", string.Format("アカウント:{0}のメールアドレスの確認を完了しました。",result.UserName), true);
            }
            else
            {
                return sendMailConfirmResult("エラー", result.ErrorMessage, true);

            }
        }

         private ActionResult loginFailedResult(string errorMsg)
        {
            return View("LoginFailed", new SimpleMessageResponse(errorMsg));
        }

        private ActionResult accountCreationStateResult(string errorMsg, bool isPositive)
        {
            return RedirectToAction("AddAccount", "Account", new AddAccountInfomationResponse(errorMsg, isPositive));
        }

        private ActionResult sendMailConfirmResult(string header, string message, bool isPositive)
        {
            return View("SendMailConfirm", new SendMailConfirmationResponse()
            {
                Header = header,
                Message = message,
                Success = isPositive
            });
        }

    }

    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class SendMailConfirmationResponse
    {
        public SendMailConfirmationResponse()
        {
            
        }
        public bool Success { get; set; }

        public string Header { get; set; }
        [AllowHtml]
        public string Message { get; set; }
    }

    public class AddAccountInfomationResponse
    {

        public AddAccountInfomationResponse()
        {
            
        }
        public AddAccountInfomationResponse(string infomationMessage, bool isPositiveMessage)
        {
            InfomationMessage = infomationMessage;
            IsPositiveMessage = isPositiveMessage;
        }
        [AllowHtml]
        public string InfomationMessage { get; set; }

        public bool IsPositiveMessage { get; set; }
    }

    public class AddAccountRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }
    }
}