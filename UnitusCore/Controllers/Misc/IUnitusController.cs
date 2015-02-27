using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Controllers.Misc
{
    public interface IUnitusController
    {
        IOwinContext OwinContext { get; }

        ApplicationUser CurrentUser { get; }

        ApplicationDbContext DbSession { get; }

        ApplicationUserManager UserManager { get; }

        IAuthenticationManager AuthenticationContext { get; }
    }
}