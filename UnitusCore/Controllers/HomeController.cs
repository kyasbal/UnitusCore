using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
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

        public ApplicationDbContext DbContext
        {
            get { return Request.GetOwinContext().Get<ApplicationDbContext>(); }
        }

        [HttpGet]
        [Authorize]
        // GET: Home
        public ActionResult Index(DashboardRequest req=null)
        {
            req = req ?? new DashboardRequest(null);
            var permissionManager=Request.GetOwinContext().GetPermissionManager();
            var user=UserManager.FindByName(User.Identity.Name);
            DbContext.Entry(user).Reference(p => p.PersonData);
            return View(new DashboardResponse(req.DashboardInformations,user,permissionManager.CheckPermission("Administrator",User.Identity.Name),AjaxRequestExtension.GetAjaxValidToken()));
        }
    }

    public class DashboardResponse
    {
        public DashboardResponse(DashboardInformation infos, ApplicationUser targetUser,  bool isAdministrator, string validationAjaxToken)
        {
            Informations = infos;
            TargetUser = targetUser;
            IsAdministrator = isAdministrator;
            ValidationAjaxToken = validationAjaxToken;
        }

        public bool IsAdministrator;

        public DashboardInformation Informations;

        public ApplicationUser TargetUser;

        public string ValidationAjaxToken { get; set; }
    }

    public class DashboardRequest
    {
        public DashboardRequest()
        {
            
        }
        public DashboardRequest(DashboardInformation dashboardInformations)
        {
            DashboardInformations = dashboardInformations;
        }

        public DashboardInformation DashboardInformations { get; set; }
    }

    public class DashboardInformation
    {
        public DashboardInformation()
        {
            
        }
        public DashboardInformation(string type, string message, string title)
        {
            this.type = type;
            this.message = message;
            this.title = title;
        }
        public string type { get; set; }

        public string title { get; set; }

        public string message { get; set; }
    }
}