using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using UnitusCore.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Controllers
{
    public class UnitusApiController : ApiController
    {
        public IOwinContext OwinContext
        {
            get { return Request.GetOwinContext(); }
        }

        private ApplicationUser currentUserData;

        public ApplicationUser CurrentUser
        {
            get
            {
                currentUserData = currentUserData ?? UserManager.FindByName(User.Identity.Name);
                return currentUserData;
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

    }
}
