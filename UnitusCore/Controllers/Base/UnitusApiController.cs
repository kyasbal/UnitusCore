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
    /// <summary>
    /// ApiControllerのUnitus用継承
    /// IUnitusApiControllerのWeb API 2.0実装
    /// </summary>
    public class UnitusApiController : ApiController,IUnitusController
    {
        private ApplicationUser currentUserCache;
        public ControllerEnsure Ensure { get; private set; }

        public UnitusApiController()
        {
            Ensure=new ControllerEnsure(this);
            MapperHelper.Initialize();
        }

        /// <summary>
        /// Owinコンテキスト
        /// </summary>
        public IOwinContext OwinContext
        {
            get { return Request.GetOwinContext(); }
        }
        /// <summary>
        /// 現在のユーザー(ログインしていない場合はnull)
        /// </summary>
        public ApplicationUser CurrentUser
        {
            get
            {
                currentUserCache = currentUserCache ?? UserManager.FindByName(User.Identity.Name);
                return currentUserCache;
            }
        }
        /// <summary>
        /// 現在のユーザー。プロフィール情報が必要な場合。(ログインしていない場合はnull)
        /// プロフィール情報も取得しようとします。必要ない場合はパフォーマンスの向上のためCurrentUserを用いるべきでしょう。
        /// </summary>
        public ApplicationUser CurrentUserWithPerson
        {
            get
            {
                if (currentUserCache==null||currentUserCache.PersonData == null)
                {
                    if (currentUserCache != null)
                    {
                        currentUserCache.LoadPersonData(DbSession).RunSynchronously();
                        return currentUserCache;
                    }
                    currentUserCache =
                        DbSession.Users.Include(a => a.PersonData)
                            .Where(a => a.UserName.Equals(User.Identity.Name))
                            .FirstOrDefault();
                }
                return currentUserCache;
            }
        }
        /// <summary>
        /// 現在のDB接続クラスへの参照(ApplicationDbContext)
        /// </summary>
        public ApplicationDbContext DbSession
        {
            get { return OwinContext.Get<ApplicationDbContext>(); }
        }
        /// <summary>
        /// ASP.Net管理のユーザー管理マネージャへの参照(ApplicationUserManager)
        /// </summary>
        public ApplicationUserManager UserManager
        {
            get { return OwinContext.GetUserManager<ApplicationUserManager>(); }
        }
        /// <summary>
        /// 認証処理に用いるAuthenticationContext
        /// </summary>
        public IAuthenticationManager AuthenticationContext
        {
            get { return OwinContext.Authentication; }
        }
        ///
        public IHttpActionResult JsonResult<T>(T content)
        {
            return Json(content);
        }

        public async Task<ApplicationUser> FindUserFromName(string userName,bool allowNotFound=false)
        {
            ApplicationUser applicationUser = await UserManager.FindByNameAsync(userName);
            if(applicationUser==null&&!allowNotFound)throw new HttpResponseException(HttpStatusCode.NotFound);
            return applicationUser;
        }

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
