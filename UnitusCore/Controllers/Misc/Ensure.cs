using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Controllers.Misc
{
    public class ControllerEnsure
    {
        private readonly IUnitusController _controller;

        public ControllerEnsure(IUnitusController controller)
        {
            _controller = controller;
        }

        public ApplicationUser ExistingUserFromId(string userId)
        {
            ApplicationUser user = _controller.DbSession.Users.Find(userId);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }

        public async Task<ApplicationUser> ExistingUserFromEmail(string email)
        {
            ApplicationUser user = await _controller.UserManager.FindByNameAsync(email);
            if (user == null) throw new HttpResponseException(HttpStatusCode.NotFound);
            return user;
        }
    }
}