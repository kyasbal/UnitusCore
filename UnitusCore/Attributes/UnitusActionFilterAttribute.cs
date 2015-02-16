using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Attributes
{
    public class UnitusActionFilterAttribute:ActionFilterAttribute
    {
        private IOwinContext _owinContext;

        protected IOwinContext GetOwinContext(HttpActionContext context)
        {
            _owinContext = _owinContext ?? context.Request.GetOwinContext();
            return _owinContext;
        }

        private ApplicationDbContext _dbContext;

        protected ApplicationDbContext GetDbContext(HttpActionContext context)
        {
            _dbContext = _dbContext ?? GetOwinContext(context).Get<ApplicationDbContext>();
            return _dbContext;
        }

        private ApplicationUserManager _userManager;

        protected ApplicationUserManager GetUserManager(HttpActionContext context)
        {
            _userManager = _userManager ?? GetOwinContext(context).GetUserManager<ApplicationUserManager>();
            return _userManager;
        }

        private ApplicationUser _currentUser;

        protected ApplicationUser GetCurrentUser(HttpActionContext context, bool needPersonData = false,bool needPermissionData=false)
        {
            _currentUser = _currentUser ?? GetUserManager(context).FindByName(HttpContext.Current.User.Identity.Name);
            
            if (needPersonData)
            {
                var personState = GetDbContext(context).Entry(_currentUser).Reference(a => a.PersonData);
                if (!personState.IsLoaded)
                {
                    personState.Load();
                }
            }
            if (needPermissionData)
            {
                var permissionState = GetDbContext(context).Entry(_currentUser).Collection(a => a.Permissions);
                if (!permissionState.IsLoaded)
                {
                    permissionState.Load();
                }
            }
            return _currentUser;
        }

        protected bool IsAdmin(HttpActionContext context)
        {
            PermissionManager permission = new PermissionManager(GetDbContext(context), GetUserManager(context));
            return permission.CheckPermission(GlobalConstants.AdminRoleName, GetCurrentUser(context).UserName);
        }
    }
}