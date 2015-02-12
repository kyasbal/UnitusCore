using System;
using System.Collections.Generic;
using System.Data.Entity;
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
// ReSharper disable ReplaceWithSingleCallToFirstOrDefault

namespace UnitusCore.Controllers
{
    public class UnitusApiController : ApiController
    {
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
                if (currentUserCache?.PersonData == null)
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

    }
}
