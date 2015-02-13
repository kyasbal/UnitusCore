using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Octokit;
using Octokit.Internal;
using UnitusCore.Models;

namespace UnitusCore.Util
{
    public class GithubAssociationManager
    {
        private ApplicationDbContext applicationDbContext;

        private ApplicationUserManager userManager;

        private ProductHeaderValue applicationHeaderValue;

        public GithubAssociationManager(ApplicationDbContext applicationDbContext, ApplicationUserManager userManager)
        {
            this.applicationDbContext = applicationDbContext;
            this.userManager = userManager;
            this.applicationHeaderValue=new ProductHeaderValue("UNITUS-CORE","1.00");
        }

        public GitHubClient GetAuthenticatedClient(string userName)
        {
            var user = userManager.FindByName(userName);
            var github = new GitHubClient(applicationHeaderValue,
                new InMemoryCredentialStore(new Credentials(user.GithubAccessToken)));
            return github;
        }


    }
}