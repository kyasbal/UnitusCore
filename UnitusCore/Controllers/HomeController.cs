using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Models;

namespace UnitusCore.Controllers
{
    
    public class HomeController : Controller
    {
        public ApplicationUserManager UserManager
        {
            get { return Request.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
        }

        public IAuthenticationManager AuthenticationContext
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        [HttpGet]
        [Authorize]
        // GET: Home
        public ActionResult Index()
        {
            var permissionManager=Request.GetOwinContext().GetPermissionManager();
            var user=UserManager.FindByName(User.Identity.Name);
            return View(new DashboardResponse(user,"",permissionManager.CheckPermission("Administrator",User.Identity.Name),AjaxRequestExtension.GetAjaxValidToken()));
        }
    }

    public class DashboardResponse
    {
        public DashboardResponse(ApplicationUser targetUser, string information, bool isAdministrator, string validationAjaxToken)
        {
            TargetUser = targetUser;
            Information = information;
            IsAdministrator = isAdministrator;
            ValidationAjaxToken = validationAjaxToken;
        }

        public bool IsAdministrator;

        public ApplicationUser TargetUser;

        public string Information;

        public string ValidationAjaxToken { get; set; }
    }
}