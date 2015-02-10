using System.Collections.Generic;
using UnitusCore.Models.BaseClasses;

namespace UnitusCore.Models.DataModel
{
    public class UserPermission : ModelBase
    {
        public UserPermission()
        {
            AllowedUsers = new HashSet<ApplicationUser>();
        }
        public string PermissionName { get; set; }

        public ICollection<ApplicationUser> AllowedUsers { get; set; }//binded
    }
}