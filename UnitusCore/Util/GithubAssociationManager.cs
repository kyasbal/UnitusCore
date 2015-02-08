using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using UnitusCore.Models;

namespace UnitusCore.Util
{
    public class GithubAssociationManager
    {
        private ApplicationDbContext applicationDbContext;

        private ApplicationUserManager userManager;

        public GithubAssociationManager(ApplicationDbContext applicationDbContext, ApplicationUserManager userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
        }

        //public HttpClient GetAuthenticatedClient(string userName)
        //{
        //    var user=userManager.FindByName(userName);

        //}


    }
}