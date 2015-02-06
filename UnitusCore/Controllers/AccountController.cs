using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Permissions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Attributes;
using UnitusCore.Models;

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
                ApplicationUser user=null;
                if ((user = UserManager.Find(request.UserName, request.Password)) != null)
                {
                    var loginIdentity =
                        await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationContext.SignIn(new AuthenticationProperties() {IsPersistent = true}, loginIdentity);
                    return RedirectToAction("Index","Home");
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

        [RoleRestrict("Administrator")]
        [HttpGet]
        public async Task<ActionResult> AddAcount()
        {
            return View();
        }

        private ActionResult loginFailedResult(string errorMsg)
        {
            return View("LoginFailed", new SimpleMessageResponse(errorMsg));
        }

    }

    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}