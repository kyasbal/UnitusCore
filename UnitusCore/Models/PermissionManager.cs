using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Models
{
    public class PermissionManager
    {
        private ApplicationDbContext _dbContext;

        private ApplicationUserManager _userManager;

        public PermissionManager(ApplicationDbContext dbContext, ApplicationUserManager userManager)
        {
            this._dbContext = dbContext;
            _userManager = userManager;
        }

        /// <summary>
        /// 指定したPermission名が存在するか確認します。
        /// </summary>
        /// <param name="permissionName"></param>
        /// <returns></returns>
        public bool IsInPermissions(string permissionName)
        {
            var permission=_dbContext.Permissions.FirstOrDefault(d => d.PermissionName.Equals(permissionName));
            return permission != null;
        }

        /// <summary>
        /// Permissionの全リストを返却します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserPermission>  GetPermissions()
        {
            return _dbContext.Permissions;
        }

        /// <summary>
        /// パーミッションを取得します。
        /// 存在しない場合は作成されます。
        /// </summary>
        /// <param name="permissionName"></param>
        public UserPermission SafePermissionGet(string permissionName)
        {
            if (!IsInPermissions(permissionName))
            {
                UserPermission newPermission = new UserPermission();
                newPermission.GenerateId();
                newPermission.PermissionName = permissionName;
                _dbContext.Permissions.Add(newPermission);
                _dbContext.SaveChanges();
                return newPermission;
            }
            else
            {
                return _dbContext.Permissions.FirstOrDefault(a => a.PermissionName.Equals(permissionName));
            }
        }

        /// <summary>
        /// 指定したユーザーが指定したパーミッションを持っているかチェックします。
        /// </summary>
        /// <param name="permission"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool CheckPermission(string permission,string userName)
        {
            
            ApplicationUser user =_userManager.FindByName(userName);
            if (user == null) return false;
            else
            {
                _dbContext.Entry(user).Collection(a => a.Permissions).Load();
                return user.Permissions.Any(c => c.PermissionName.Equals(permission));
            }
        }


        public async void ApplyPermissionToUser(string permissionName, string userName)
        {
            ApplicationUser user =  _userManager.FindByName(userName);
            if (user == null)throw  new InvalidDataException("username not found");
            else
            {
                UserPermission permission = SafePermissionGet(permissionName);
                 _dbContext.Entry(permission).Collection(a => a.AllowedUsers).Load();
                 _dbContext.Entry(user).Collection(a => a.Permissions).Load();
                permission.AllowedUsers.Add(user);
                user.Permissions.Add(permission);
                _dbContext.SaveChanges();
            }
        }

        public async Task DetachPermissionFromUser(string permissionName, string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            if (user == null) throw new InvalidDataException("username not found");
            else
            {
                UserPermission permission = SafePermissionGet(permissionName);
                await _dbContext.Entry(permission).Collection(a => a.AllowedUsers).LoadAsync();
                await _dbContext.Entry(user).Collection(a => a.Permissions).LoadAsync();
                if(permission.AllowedUsers.Contains(user))permission.AllowedUsers.Remove(user);
                if(user.Permissions.Contains(permission))user.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
            }
        }

    }

    public static class PermissonManagerExtension
    {
        public static PermissionManager GetPermissionManager(this IOwinContext context)
        {
            return new PermissionManager(context.Get<ApplicationDbContext>(),context.GetUserManager<ApplicationUserManager>());
        }
        
         
    }
}