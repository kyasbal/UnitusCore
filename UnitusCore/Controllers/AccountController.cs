using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;

namespace UnitusCore.Controllers
{
    public class AccountController : Controller
    {
        public ApplicationUserManager UserManager
        {
            get { return Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginRequest request)
        {
            var userIdentity=await UserManager.CreateAsync(new ApplicationUser() {Id = Guid.NewGuid().ToString(),UserName = request.UserName}, request.Password);
            return View("LoginFailed", new SimpleMessageResponse(userIdentity.Errors.FirstOrDefault()));
        }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}