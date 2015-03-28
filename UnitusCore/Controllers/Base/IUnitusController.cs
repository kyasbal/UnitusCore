using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Controllers.Base
{
    /// <summary>
    /// Controller及びApiControllerのUnitusController/UnitusApiControllerへの共通拡張インターフェース
    /// </summary>
    public interface IUnitusController
    {
        /// <summary>
        /// 現状のOwinContext
        /// </summary>
        IOwinContext OwinContext { get; }

        /// <summary>
        /// 現在のユーザー(ログインしていない場合はnull)
        /// </summary>
        ApplicationUser CurrentUser { get; }

        /// <summary>
        /// 現在のデータベース接続(ApplicationDbContext)
        /// </summary>
        ApplicationDbContext DbSession { get; }
        
        /// <summary>
        /// 現在のユーザーマネージャ(ログインの管理など)
        /// </summary>
        ApplicationUserManager UserManager { get; }

        /// <summary>
        /// 現在の権限マネージャ。ApplicationUserManagerと併用することが多い。
        /// </summary>
        IAuthenticationManager AuthenticationContext { get; }
    }
}