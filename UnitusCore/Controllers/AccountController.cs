using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Permissions;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SendGridSharp;
using UnitusCore.Attributes;
using UnitusCore.Controllers.Base;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{
    public class AccountController : UnitusController
    {
        [RequireHttps]  
        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        // GET: Account
        public ActionResult Login(string ReturnUrl=null)
        {
            Session["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
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
                    if (Session["ReturnUrl"] != null) return Redirect((string) Session["ReturnUrl"]);
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

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> LoginWithAdmin(LoginRequest request)
        {
            if(request.UserName!="LimeStreem@gmail.com")throw new HttpResponseException(HttpStatusCode.BadRequest);
            if (ModelState.IsValid)
            {
                ApplicationUser user = null;
                if ((user = UserManager.Find(request.UserName, request.Password)) != null)
                {
                    var loginIdentity =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationContext.SignIn(new AuthenticationProperties() { IsPersistent = true }, loginIdentity);
                    if (Session["ReturnUrl"] != null) return Redirect((string)Session["ReturnUrl"]);
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

        [System.Web.Mvc.Authorize]
        [RoleRestrict("Administrator")]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> AddAccount(AddAccountInfomationResponse response=null)
        {
            return View(response);
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpPost]
        [RoleRestrict("Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAccount(AddAccountRequest request)
        {
            if (ModelState.IsValid)
            {
                return CreateUnitusAccount(request);
            }
            else
            {
                return
                    accountCreationStateResult(
                        string.Format("アカウント:{0}の作成に失敗しました。<br>不正なパラメータが渡されました。", request.UserName), false);
            }
        }

        internal ActionResult CreateUnitusAccount(AddAccountRequest request)
        {
            var state = UserManager.CreateUser(DbSession,request.UserName, request.Password);
            if (state.Succeeded)
            {
                if (request.IsAdmin)
                {
                    Request.GetOwinContext()
                        .GetPermissionManager()
                        .ApplyPermissionToUser(GlobalConstants.AdminRoleName, request.UserName);
                }
                var user = UserManager.FindByName(request.UserName);
                var result = MailConfirmationManager.SendMailConfirmation(user, this);
                if (result.Success)
                {
                    return
                        accountCreationStateResult(
                            string.Format("アカウント:{0}は正常に作成されました。<br>メールアドレスの確認のためのメールを{0}に送りました。", request.UserName), true);
                }
                else
                {
//メールが遅れなかった場合
                    return accountCreationStateResult(string.Format("アカウント:{0}は正常に作成されました。", request.UserName), true);
                }
            }
            else
            {
                return
                    accountCreationStateResult(
                        string.Format("アカウント:{0}の作成に失敗しました。<br>{1}", request.UserName, state.Errors.FirstOrDefault()), false);
            }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpGet]
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

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
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

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> ForgotPassword(string errorMsg=null)
        {
            return View("ForgotPassword",new PasswordForgotResponse() {ErrorMessage = errorMsg});
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(PasswordForgotRequest req)
        {
            var result=PasswordForgotManager.SendPasswordResetMail(req.EmailAddress, this);
            if (result.Success)
            {
                return View("ForgotPasswordMailSent");
            }
            else
            {
                return await ForgotPassword(result.ErrorMessage);
            }
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> ForgotPasswordConfirm(string confirmId)
        {
            return await ForgotPasswordConfirm(new PasswordInputRequest() {confirmId = confirmId});
        }

        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        public async Task<ActionResult> ForgotPasswordConfirm(PasswordInputRequest req)
        {
            var result = PasswordForgotManager.ConfirmConfirmationId(req.confirmId, this,false);
            if (result.Success)
            {
                return View(req);
            }
            else
            {
                return await ForgotPassword("パスワードリセットトークンの認証に失敗しました。<br>"+result.ErrorMessage);
            }
        }

        [System.Web.Mvc.Authorize]
        [System.Web.Mvc.HttpGet]
        public async Task<ActionResult> Logout()
        {
            AuthenticationContext.SignOut("ApplicationCookie");
            return Redirect("/Account/Login");
        }


        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPasswordConfirmSave(PasswordForgotConfirmRequest req)
        {
            var result = PasswordForgotManager.ConfirmConfirmationId(req.confirmId, this,true);
            if (result.Success)
            {
                var user = UserManager.FindByName(result.UserName);
                if (user != null)
                {
                    IdentityResult resetResult=UserManager.ResetPassword(user.Id, req.confirmId, req.Password);
                    if (resetResult.Succeeded)
                    {
                        SendGridManager.SendUseTemplate(user.UserName,TemplateType.PasswordResetConfirmationCompleted,new Dictionary<string, string>()
                        {
                            {"-username-",user.Email}
                        });
                        return View("ForgotPasswordComplete");
                    }
                    var request = new PasswordInputRequest()
                    {
                        confirmId = req.confirmId,
                        ErrorMessage = resetResult.Errors.FirstOrDefault()
                    };
                    if (request.ErrorMessage == null || string.IsNullOrWhiteSpace(request.ErrorMessage))
                        request.ErrorMessage = "パスワードの更新に失敗しました。";
                    return RedirectToAction("ForgotPasswordConfirm", request);
                }
                else
                {
                    return await ForgotPassword("パスワードリセットトークンの認証に失敗しました。<br>" + result.ErrorMessage);
                }
            }
            else
            {
                return await ForgotPassword("パスワードリセットトークンの認証に失敗しました。<br>" + result.ErrorMessage);
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

    public class PasswordForgotRequest
    {
        public string EmailAddress { get; set; }
    }

    public class PasswordForgotResponse
    {
        [AllowHtml]
        public string ErrorMessage { get; set; }
    }

    public class PasswordInputRequest
    {
        public string confirmId { get; set; }

        public string ErrorMessage { get; set; }
    }

    public class PasswordForgotConfirmRequest
    {
        public string confirmId { get; set; }
        public string Password { get; set; }
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
        [EmailAddress]
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool IsAdmin { get; set; }

        public string Name { get; set; }
    }
}