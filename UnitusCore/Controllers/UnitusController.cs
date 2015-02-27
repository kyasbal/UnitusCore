using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using UnitusCore.Controllers.Misc;
using UnitusCore.Models;
using UnitusCore.Models.DataModel;

namespace UnitusCore.Controllers
{
    public class UnitusController : Controller,IUnitusController
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