using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using UnitusCore.Models;

namespace UnitusCore.Attributes
{
    public class RoleRestrictAttribute : ActionFilterAttribute,IActionFilter
    {
        private string _roleName;

        public RoleRestrictAttribute(string roleName)
        {
            this._roleName = roleName;
        }

        void  IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            var permissionManager =
                filterContext.RequestContext.HttpContext.GetOwinContext().GetPermissionManager();
            var currentUser = filterContext.RequestContext.HttpContext.GetOwinContext().Authentication.User;
            if (currentUser != null)
            {
                
                if (!permissionManager.CheckPermission(_roleName, currentUser.Identity.Name))
                {
                    filterContext.Result=new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
            }
            else
            {
                filterContext.Result = new HttpUnauthorizedResult("このページにアクセスする前にログインする必要があります。");
            }

            base.OnActionExecuting(filterContext);
        }

        //public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        //{
        //    var currentUser = actionContext.Request.GetOwinContext().Authentication.User;
        //    if (currentUser != null)
        //    {
        //        if (!currentUser.IsInRole(_roleName))
        //        {
        //            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //            return Task.FromResult(0);
        //        }
        //    }
        //    else
        //    {
        //        actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //        return Task.FromResult(0);
        //    }

        //    return base.OnActionExecutingAsync(actionContext, cancellationToken);
        //}

        //public override void OnActionExecuting(HttpActionContext actionContext)
        //{
        //    var appContext = actionContext.Request.GetOwinContext().Get<ApplicationDbContext>();
        //    var currentUser=actionContext.Request.GetOwinContext().Authentication.User;
        //    if (currentUser != null)
        //    {
        //        if (!currentUser.IsInRole(_roleName))
        //        {
        //            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //        }
        //    }
        //    else
        //    {
        //        actionContext.Response=new HttpResponseMessage(HttpStatusCode.Unauthorized);
        //    }

        //    base.OnActionExecuting(actionContext);
        //}
    }

}