using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Controllers.Base;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;
using UnitusCore.Storage.Base;

// ReSharper disable ReplaceWithSingleCallToFirstOrDefault

namespace UnitusCore.Controllers
{
    public class UnitusApiController : ApiController,IUnitusController
    {
        public UnitusApiController()
        {
            Ensure=new ControllerEnsure(this);
            MapperHelper.Initialize();
        }

        public IOwinContext OwinContext
        {
            get { return Request.GetOwinContext(); }
        }

        protected ApplicationUser currentUserCache;

        public ApplicationUser CurrentUser
        {
            get
            {
                currentUserCache = currentUserCache ?? UserManager.FindByName(User.Identity.Name);
                return currentUserCache;
            }
        }

        public ApplicationUser CurrentUserWithPerson
        {
            get
            {
                if (currentUserCache==null||currentUserCache.PersonData == null)
                {
                    currentUserCache =
                        DbSession.Users.Include(a => a.PersonData)
                            .Where(a => a.UserName.Equals(User.Identity.Name))
                            .FirstOrDefault();
                }
                return currentUserCache;
            }
        }

        public ApplicationDbContext DbSession
        {
            get { return OwinContext.Get<ApplicationDbContext>(); }
        }

        public ApplicationUserManager UserManager
        {
            get { return OwinContext.GetUserManager<ApplicationUserManager>(); }
        }

        public IAuthenticationManager AuthenticationContext
        {
            get { return OwinContext.Authentication; }
        }

        public IHttpActionResult JsonResult<T>(T content)
        {
            return Json(content);
        }

        public ControllerEnsure Ensure { get; private set; }

        public async Task<ApplicationUser> FindUserFromName(string userName,bool allowNotFound=false)
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(userName);
            if(applicationUser==null&&!allowNotFound)throw new HttpResponseException(HttpStatusCode.NotFound);
            return applicationUser;
        }
    }

    public class UnitusApiControllerWithTableConnection:UnitusApiController
    {
        private TableStorageConnection _tableConnection;

        protected TableStorageConnection TableConnection
        {
            get
            {
                _tableConnection = _tableConnection ?? new TableStorageConnection();
                return _tableConnection;
            }
        }
    }
}
