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
using UnitusCore.Models.DataModel;
using UnitusCore.Util;

namespace UnitusCore.Controllers
{

    public class HomeController : UnitusController
    {
        [HttpGet]
        [Authorize]
        // GET: Home
        public ActionResult Index()
        {
            if (string.IsNullOrWhiteSpace(CurrentUser.GithubAccessToken))
            {
                this.AddNotification(NotificationType.Error, "Github連携が未設定です",
                    string.Format("<a href=\"{0}\">こちら</a>をクリックして設定してください。", Url.Action("Authorize", "Github")), false);
            }

            var permissionManager = Request.GetOwinContext().GetPermissionManager();
            var user = UserManager.FindByName(User.Identity.Name);
            DbSession.Entry(user).Reference(p => p.PersonData);
            return
                View(new DashboardResponse(this.GetCurrentDashboardRequest(true).DashboardInformations.ToArray(), user,
                    permissionManager.CheckPermission("Administrator", User.Identity.Name),
                    AjaxRequestExtension.GetAjaxValidToken()));
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Cert()
        {
            return Content("ckknF3Jp4XynZBxEn4Em");
        }
    }


    public static class DashboardExtension
    {
        public static void AddNotification(this Controller controller,NotificationType type,string title,string message,bool timeDismission=true)
        {
            DashboardRequest req = controller.Session["Dashboard-Request"] as DashboardRequest;
            req = req ?? new DashboardRequest();
            string typeStr = "";
            switch (type)
            {
                case NotificationType.Success:
                    typeStr = "success";
                    break;
                case NotificationType.Warning:
                    typeStr = "warning";
                    break;
                case NotificationType.Error:
                    typeStr = "error";
                    break;
                case NotificationType.Info:
                    typeStr = "info";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
            req.DashboardInformations.Add(new DashboardInformation(typeStr,title,message,timeDismission));
            controller.Session["Dashboard-Request"] = req;
        }

        public static DashboardRequest GetCurrentDashboardRequest(this Controller controller,bool remove=false)
        {
            DashboardRequest req = controller.Session["Dashboard-Request"] as DashboardRequest;
            req = req ?? new DashboardRequest();
            if (remove)
            {
                controller.Session["Dashboard-Request"] = null;
            }
            return req;
        }
    }

    public enum NotificationType
    {
        Success,
        Warning,
        Error,
        Info
    }

    public class DashboardResponse
    {
        public DashboardResponse(DashboardInformation[] infos, ApplicationUser targetUser,  bool isAdministrator, string validationAjaxToken)
        {
            Informations = infos;
            TargetUser = targetUser;
            IsAdministrator = isAdministrator;
            ValidationAjaxToken = validationAjaxToken;
        }

        public bool IsAdministrator;

        public DashboardInformation[] Informations;

        public ApplicationUser TargetUser;

        public string ValidationAjaxToken { get; set; }
    }

    public class DashboardRequest
    {
        public DashboardRequest()
        {
            DashboardInformations=new List<DashboardInformation>();
        }
        public DashboardRequest(List<DashboardInformation> dashboardInformations)
        {
            DashboardInformations = dashboardInformations;
        }

        public List<DashboardInformation> DashboardInformations { get; set; }
    }

    public class DashboardInformation
    {
        public DashboardInformation()
        {
            
        }
        public DashboardInformation(string type, string title, string message,bool timeDismission)
        {
            this.type = type;
            this.message = message;
            this.title = title;
            this.timeDismission = timeDismission;
        }
        public string type { get; set; }

        public string title { get; set; }

        public string message { get; set; }

        public bool timeDismission { get; set; }
    }
}